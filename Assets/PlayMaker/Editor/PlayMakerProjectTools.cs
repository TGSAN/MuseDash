#if (UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
#define UNITY_PRE_5_3
#endif

#if (UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7)
#define UNITY_PRE_5_0
#endif

using System;
using System.IO;
using System.Reflection;
using HutongGames.PlayMaker;
using UnityEditor;
#if !UNITY_PRE_5_3
using UnityEditor.SceneManagement;
#endif
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    public class ProjectTools
    {
        // Change MenuRoot to move the Playmaker Menu
        // E.g., MenuRoot = "Plugins/PlayMaker/"
        private const string MenuRoot = "PlayMaker/";

        [MenuItem(MenuRoot + "Tools/Update All Loaded FSMs", false, 25)]
        public static void UpdateAllLoadedFSMs()
        {
            ReSaveAllLoadedFSMs();
        }

        [MenuItem(MenuRoot + "Tools/Update All FSMs in Build", false, 26)]
        public static void UpdateAllFSMsInBuild()
        {
            UpdateScenesInBuild();
        }

        /*WIP
        [MenuItem(MenuRoot + "Tools/Scan Scenes", false, 33)]
        public static void ScanScenesInProject()
        {
            FindAllScenes();
        }
*/

        private static void ReSaveAllLoadedFSMs()
        {
            Debug.Log("Checking loaded FSMs...");
            FsmEditor.RebuildFsmList();
            foreach (var fsm in FsmEditor.FsmList)
            {
                // Re-initialize loads data and forces a dirty check
                // so we can just call this and let it handle dirty etc.

                fsm.Reinitialize();
            }
        }

        private static void UpdateScenesInBuild()
        {
            // Allow the user to save his work!
#if UNITY_PRE_5_3
            if (!EditorApplication.SaveCurrentSceneIfUserWantsTo())
#else
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
#endif
            {
                return;
            }

            LoadPrefabsWithPlayMakerFSMComponents();

            foreach (var scene in EditorBuildSettings.scenes)
            {
                Debug.Log("Open Scene: " + scene.path);
#if UNITY_PRE_5_3
                EditorApplication.OpenScene(scene.path);
#else
                EditorSceneManager.OpenScene(scene.path);
#endif
                
#if UNITY_5                
                UpdateGetSetPropertyActions();
#endif
                                
                ReSaveAllLoadedFSMs();

#if UNITY_PRE_5_3
                if (!EditorApplication.SaveScene())
#else
                if (!EditorSceneManager.SaveOpenScenes())
#endif
                {
                    Debug.LogError("Could not save scene!");
                }
            }
        }

        private static void LoadPrefabsWithPlayMakerFSMComponents()
        {
            Debug.Log("Finding Prefabs with PlayMakerFSMs");

            var searchDirectory = new DirectoryInfo(Application.dataPath);
            var prefabFiles = searchDirectory.GetFiles("*.prefab", SearchOption.AllDirectories);

            foreach (var file in prefabFiles)
            {
                var filePath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                //Debug.Log(filePath + "\n" + Application.dataPath);

                var dependencies = AssetDatabase.GetDependencies(new[] { filePath });
                foreach (var dependency in dependencies)
                {
                    if (dependency.Contains("/PlayMaker.dll"))
                    {
                        Debug.Log("Found Prefab with FSM: " + filePath);
                        AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
                    }
                }
            }

            FsmEditor.RebuildFsmList();
        }

#if UNITY_5

        private static void UpdateGetSetPropertyActions()
        {
            var getPropertyActionType = ActionData.GetActionType("HutongGames.PlayMaker.Actions.GetProperty");
            var setPropertyActionType = ActionData.GetActionType("HutongGames.PlayMaker.Actions.SetProperty");

            FsmEditor.RebuildFsmList();

            foreach (var fsm in FsmEditor.FsmList)
            {
                if (fsm.GameObject == null) continue; // can't update property paths without GameObject

                var upgraded = false;
                foreach (var state in fsm.States)
                {
                    foreach (var action in state.Actions)
                    {
                        var actionType = action.GetType();
                        if (actionType == getPropertyActionType)
                        {
                            var targetPropertyField = getPropertyActionType.GetField("targetProperty", BindingFlags.Public | BindingFlags.Instance);
                            if (targetPropertyField != null)
                            {
                                upgraded |= TryUpgradeFsmProperty(fsm.GameObject, targetPropertyField.GetValue(action) as FsmProperty);
                            }
                        }
                        else if (actionType == setPropertyActionType)
                        {
                            var targetPropertyField = setPropertyActionType.GetField("targetProperty", BindingFlags.Public | BindingFlags.Instance);
                            if (targetPropertyField != null)
                            {
                                upgraded |= TryUpgradeFsmProperty(fsm.GameObject, targetPropertyField.GetValue(action) as FsmProperty);
                            }
                        }
                    }
                }
                if (upgraded)
                {
                    //Undo called in batch operation seems to crash Unity
                    //FsmEditor.SaveActions(fsm);

                    foreach (var state in fsm.States)
				    {
					    state.SaveActions();
				    }

				    FsmEditor.SetFsmDirty(fsm, true);

#if !UNITY_PRE_5_3
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(fsm.Owner.gameObject.scene);
#elif !UNITY_PRE_5_0
                    // Not sure if we need to do this...?
                    UnityEditor.EditorApplication.MarkSceneDirty();
#endif
                }
            }
        }

        private static bool TryUpgradeFsmProperty(GameObject gameObject, FsmProperty fsmProperty)
        {
            if (gameObject == null || fsmProperty == null) return false;
            var propertyPath = fsmProperty.PropertyName;
            if (string.IsNullOrEmpty(propertyPath)) return false;

            var parts = propertyPath.Split('.');
            if (TryFindComponent(gameObject, fsmProperty, parts[0]))
            {
                var oldPath = fsmProperty.PropertyName;
                fsmProperty.PropertyName = string.Join(".", parts, 1, parts.Length-1);
                Debug.Log("Fixed: " + oldPath + "->" + fsmProperty.PropertyName);
                return true;
            }

            return false;
        }

        private static bool TryFindComponent(GameObject gameObject, FsmProperty fsmProperty, string component)
        {
            if (component == "rigidbody") return FixFsmProperty(gameObject, fsmProperty, typeof(Rigidbody));
            if (component == "rigidbody2D") return FixFsmProperty(gameObject, fsmProperty, typeof(Rigidbody2D));
            if (component == "camera") return FixFsmProperty(gameObject, fsmProperty, typeof(Camera));
            if (component == "light") return FixFsmProperty(gameObject, fsmProperty, typeof(Light));
            if (component == "animation") return FixFsmProperty(gameObject, fsmProperty, typeof(Animation));
            if (component == "constantForce") return FixFsmProperty(gameObject, fsmProperty, typeof(ConstantForce));
            if (component == "renderer") return FixFsmProperty(gameObject, fsmProperty, typeof(Renderer));
            if (component == "audio") return FixFsmProperty(gameObject, fsmProperty, typeof(AudioSource));
            if (component == "guiText") return FixFsmProperty(gameObject, fsmProperty, typeof(GUIText));
            if (component == "networkView") return FixFsmProperty(gameObject, fsmProperty, typeof(NetworkView));
            if (component == "guiTexture") return FixFsmProperty(gameObject, fsmProperty, typeof(GUITexture));
            if (component == "collider") return FixFsmProperty(gameObject, fsmProperty, typeof(Collider));
            if (component == "collider2D") return FixFsmProperty(gameObject, fsmProperty, typeof(Collider2D));
            if (component == "hingeJoint") return FixFsmProperty(gameObject, fsmProperty, typeof(HingeJoint));
            if (component == "particleEmitter") return FixFsmProperty(gameObject, fsmProperty, typeof(ParticleEmitter));
            if (component == "particleSystem") return FixFsmProperty(gameObject, fsmProperty, typeof(ParticleSystem));

            return false;
        }

        private static bool FixFsmProperty(GameObject gameObject, FsmProperty fsmProperty, Type componentType)
        {
            fsmProperty.TargetObject.Value = gameObject.GetComponent(componentType);
            fsmProperty.TargetObject.ObjectType = componentType;
            return true;
        }
#endif

        /* WIP
        [Localizable(false)]
        private static void FindAllScenes()
        {
            Debug.Log("Finding all scenes...");

            var searchDirectory = new DirectoryInfo(Application.dataPath);
            var assetFiles = searchDirectory.GetFiles("*.unity", SearchOption.AllDirectories);

            foreach (var file in assetFiles)
            {
                var filePath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(Object));
                if (obj == null)
                {
                    //Debug.Log(filePath + ": null!");
                }
                else if (obj.GetType() == typeof(Object))
                {
                    Debug.Log(filePath);// + ": " + obj.GetType().FullName);
                }
                //var obj = AssetDatabase.
            }
        }
         */
    }
}

