using System;
using UnityEngine;
using UnityEditor;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Stores per project settings.
    /// EditorPrefs are universal so not well suited to per project settings.
    /// NOTE: This class is included as source and cannot be accessed from dll
    /// So it's intended for use by startup and install scripts.
    /// </summary>
    [Serializable]
    public class EditorStartupPrefs : ScriptableObject
    {
        private static EditorStartupPrefs instance;

        public static EditorStartupPrefs Instance
        {
            get
            {
                if (instance == null)
                {
                    //Debug.Log("Load PlayMakerEditorPrefs");
                    instance = Resources.Load<EditorStartupPrefs>("EditorStartupPrefs");
                    if (instance == null)
                    {
                        instance = CreateInstance<EditorStartupPrefs>();
                        // There are too many edge cases where Unity isn't ready to load resources
                        // E.g., after importing a unitypackage
                        // So we won't log a message to avoid support spam.
                        //Debug.Log("Missing EditorStartupPrefs asset!");
                    }
                }
                return instance;
            }
        }

        public static string PlaymakerVersion
        {
            get { return Instance.playmakerVersion; }
            set { Instance.playmakerVersion = value; Save();}
        }

        public static string WelcomeScreenVersion
        {
            get { return Instance.welcomeScreenVersion; }
            set { Instance.welcomeScreenVersion = value; Save();}
        }

        public static bool ShowWelcomeScreen
        {
            get { return Instance.showWelcomeScreen; }
            set
            {
                if (value != Instance.showWelcomeScreen)
                {
                    Instance.showWelcomeScreen = value; 
                    Save();
                }
            }
        }

        [SerializeField] private string welcomeScreenVersion;
        [SerializeField] private string playmakerVersion;
        [SerializeField] private bool showWelcomeScreen = true;

        public static void ResetForExport()
        {
            ShowWelcomeScreen = true;
            PlaymakerVersion = string.Empty;
            WelcomeScreenVersion = string.Empty;
        }

        public static void Save()
        {
            if (!AssetDatabase.Contains(Instance))
            {
                var copy = CreateInstance<EditorStartupPrefs>();
                EditorUtility.CopySerialized(Instance, copy);
                instance = Resources.Load<EditorStartupPrefs>("EditorStartupPrefs");
                if (instance == null)
                {
                    AssetDatabase.CreateAsset(copy, "Assets/PlayMaker/Editor/Resources/EditorStartupPrefs.asset");
                    AssetDatabase.Refresh();
                    instance = copy;

                    //Debug.Log("Missing EditorStartupPrefs asset!");
                    return;
                }
                EditorUtility.CopySerialized(copy, instance);
            }
            EditorUtility.SetDirty(Instance);
        }

    }
}

