using System;
using Assets.Scripts.Common;
using EasyEditor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Editor
{
    [Groups("Create New Node", "Node Function")]
    [CustomEditor(typeof(Tool.PRHelper.PRHelper))]
    public class PRHelperEditor : EasyEditorBase
    {
        private static GUISkin m_Skin;

        public static GUISkin skin
        {
            get
            {
                if (m_Skin == null)
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        m_Skin = AssetDatabase.LoadAssetAtPath("Assets/Scripts/Tool/PRHelper/Editor/Skins/PRPRHelperSkin.guiskin", typeof(GUISkin))
                            as GUISkin;
                    }
                }
                return m_Skin;
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Box("我蕾保我代码不出错", skin.GetStyle("BoxLogo"));
            base.OnInspectorGUI();
        }

        [Inspector]
        private void NewPRNode()
        {
            var pRHelper = (PRHelper)target;

            pRHelper.isNewNode = !pRHelper.isNewNode;

            if (pRHelper.isNewNode)
            {
                ShowGroup("Create New Node");
            }
            else
            {
                HideGroup("Create New Node");
            }
        }

        [Inspector(group = "Create New Node")]
        public void CreateNode()
        {
            var pRHelper = (PRHelper)target;
            if (string.IsNullOrEmpty(pRHelper.newNodeName)) return;
            PRNodeMaker.MakerPRNode(pRHelper.newNodeName);
        }

        [Inspector(group = "Node Function", order = 0)]
        private void ShowWindow()
        {
            PRHelperWindow.Init();
        }
    }

    public class PRHelperWindow : EditorWindow
    {
        private SerializedObject m_SerializedObj;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (PRHelperWindow)EditorWindow.GetWindow(typeof(PRHelperWindow));
            window.Show();
        }

        private SerializedObject serializedObj
        {
            get
            {
                var selectedGO = Selection.activeGameObject;
                if (selectedGO == null) return null;
                var prprHelper = selectedGO.GetComponent<Tool.PRHelper.PRHelper>();
                m_SerializedObj = new SerializedObject(prprHelper);
                return m_SerializedObj;
            }
        }

        private void OnGUI()
        {
            if (m_SerializedObj == null)
            {
                m_SerializedObj = serializedObj;
            }
            m_SerializedObj.Update();
            EditorGUILayout.PropertyField(m_SerializedObj.FindProperty("nodes"));
            m_SerializedObj.ApplyModifiedProperties();
        }
    }
}