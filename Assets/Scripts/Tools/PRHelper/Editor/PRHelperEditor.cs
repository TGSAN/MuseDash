using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using Assets.Scripts.Tools.PRHelper.Properties;
using EasyEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tools.PRHelper.Editor
{
    [Serializable]
    public class PRHelperEditorNode : EditorNode
    {
        [SerializeField]
        public PRHelperNode node;

        [SerializeField]
        public List<KeyValuePair<string, List<UnityAction<string>>>> events = new List<KeyValuePair<string, List<UnityAction<string>>>>();

        public PRHelperEditorNode(int id, int wid, Rect r, PRHelperNode n)
        {
            idx = id;
            wndIdx = wid;
            rect = r;
            node = n;
        }

        public override void AddChild(EditorNode childNode, List<EditorNode> allEditorNodes, params object[] args)
        {
            base.AddChild(childNode, allEditorNodes);
            AddChildEvent(childNode, allEditorNodes, (PREvents.EventType)args[0]);
        }

        public override void RemoveChild(EditorNode childNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            base.RemoveChild(childNode, allEditorNodes);
            RemoveChildEvent(allEditorNodes, (PREvents.EventType)args[0], (int)args[1]);
        }

        public override void AddParent(EditorNode parentNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            base.AddParent(parentNode, allEditorNodes);
            var n = parentNode as PRHelperEditorNode;
            if (n == null) return;
            n.AddChildEvent(this, allEditorNodes, (PREvents.EventType)args[0]);
        }

        public override void RemoveParent(EditorNode parentNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            base.RemoveParent(parentNode, allEditorNodes);
            var n = parentNode as PRHelperEditorNode;
            if (n == null) return;
            n.RemoveChildEvent(allEditorNodes, (PREvents.EventType)args[0], (int)args[1]);
        }

        public override void Destroy(List<EditorNode> allEditorNodes)
        {
            base.Destroy(allEditorNodes);
            events.ForEach(e =>
            {
                var eventType = (PREvents.EventType)Enum.Parse(typeof(PREvents.EventType), e.Key);
                var list = e.Value;
                for (var i = 0; i < list.Count; i++)
                {
                    RemoveChildEvent(allEditorNodes, eventType, i);
                }
            });
        }

        public void AddChildEvent(EditorNode childNode, List<EditorNode> allEditorNodes, PREvents.EventType eventType)
        {
            var n = childNode as PRHelperEditorNode;
            if (n == null) return;
            var unityEvent = Tools.PRHelper.PRHelper.OnEvent(node, eventType);
            var func = Delegate.CreateDelegate(typeof(UnityAction<string>), PRHelperWindow.prHelper, typeof(Tools.PRHelper.PRHelper).GetMethod("Play")) as UnityAction<string>;
            UnityEditor.Events.UnityEventTools.AddStringPersistentListener(unityEvent, func, node.key);
            events.Find(e => e.Key == eventType.ToString()).Value.Add(func);
        }

        public void RemoveChildEvent(List<EditorNode> allEditorNodes, PREvents.EventType eventType, int pos)
        {
            var unityEvent = Tools.PRHelper.PRHelper.OnEvent(node, eventType);
            var et = events.Find(e => e.Key == eventType.ToString());
            var func = et.Value[pos];
            UnityEditor.Events.UnityEventTools.RemovePersistentListener(unityEvent, func);
            et.Value.RemoveAt(pos);
        }
    }

    [Groups("Create New Node", "Node Function")]
    [CustomEditor(typeof(Tools.PRHelper.PRHelper))]
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
                        m_Skin = AssetDatabase.LoadAssetAtPath(StringCommons.PRHelperSkinInAssets, typeof(GUISkin))
                            as GUISkin;
                    }
                }
                return m_Skin;
            }
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Box(String.Empty, skin.GetStyle("BoxLogo"));
            base.OnInspectorGUI();
        }

        [Inspector(@group = "Create New Node")]
        public void CreateNode()
        {
            var pRHelper = (Tools.PRHelper.PRHelper)target;
            if (string.IsNullOrEmpty(pRHelper.newNodeName)) return;
            PRNodeMaker.MakerPRNode(pRHelper.newNodeName);
        }

        [Inspector(@group = "Node Function", order = 0)]
        private void ShowWindow()
        {
            PRHelperWindow.Init();
        }
    }

    public class PRHelperWindow : EditorWindow
    {
        private static SerializedObject m_SerializedObj;
        private static Tools.PRHelper.PRHelper m_PRHelper;

        [SerializeField]
        private List<PRHelperEditorNode> m_EditorNodes = new List<PRHelperEditorNode>();

        private int m_SelectedNodeIdx = -1;
        private static Vector2 m_NodeWidthHeight = new Vector2(100, 65);

        private Rect m_NodeParentPointRect = new Rect(m_NodeWidthHeight.x / 2 - 9, 3, 17, 17);
        private Rect m_NodeChildPointRect = new Rect(m_NodeWidthHeight.x / 2 - 45, 48, 13, 13);

        private Rect m_VMWndRect = new Rect(100, 100, 300, 300);

        private bool m_IsNew = false;

        private GUIContent[] m_VMContents
        {
            get
            {
                var strs = Enum.GetNames(typeof(PREvents.EventType));
                return EditorUtils.GetGUIContentArray(strs);
            }
        }

        private GUIContent[] m_ViewContents
        {
            get
            {
                var strs = Enum.GetNames(typeof(NodeType)).ToList();
                strs = strs.Where(s => s.StartsWith("View_")).ToList();
                return EditorUtils.GetGUIContentArray(strs.ToArray());
            }
        }

        private GUIContent[] m_ModelContents
        {
            get
            {
                var strs = Enum.GetNames(typeof(NodeType)).ToList();
                strs = strs.Where(s => s.StartsWith("Model_")).ToList();
                return EditorUtils.GetGUIContentArray(strs.ToArray());
            }
        }

        private static GUISkin m_Skin;

        public static GUISkin skin
        {
            get
            {
                if (m_Skin == null)
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        m_Skin = AssetDatabase.LoadAssetAtPath(StringCommons.PRFormulaTreeEditorSkinPathInAssets, typeof(GUISkin)) as GUISkin;
                    }
                }
                return m_Skin;
            }
        }

        public static SerializedObject serializedObj
        {
            get
            {
                m_SerializedObj = new SerializedObject(prHelper);
                return m_SerializedObj;
            }
        }

        public static Tools.PRHelper.PRHelper prHelper
        {
            get
            {
                var selectedGO = Selection.activeGameObject;
                if (selectedGO == null) return m_PRHelper;
                m_PRHelper = selectedGO.GetComponent<Tools.PRHelper.PRHelper>();
                return m_PRHelper;
            }
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (PRHelperWindow)EditorWindow.GetWindow(typeof(PRHelperWindow), false, "PRHelper");
            window.Show();
        }

        private void OnGUI()
        {
            OnDataCheck();
            Repaint();
            BeginWindows();
            OnNodesWindow();
            EndWindows();
        }

        private void OnDataCheck()
        {
            if (m_SerializedObj == null)
            {
                m_SerializedObj = serializedObj;
            }
            /*m_SerializedObj.Update();
            m_SerializedObj.ApplyModifiedProperties();*/
        }

        private void OnNodesWindow()
        {
            GUI.Box(new Rect(0, 0, 1000, 20), "Node Graph");
            OnVMWindow();
        }

        private void OnVMWindow()
        {
            OnVMPopFuncMenu();
            var eventNodes = m_EditorNodes.Where(n => n.node.nodeType == NodeType.VM_PREvents).ToList();
            eventNodes.ForEach(eNode =>
            {
                var rectIdx = eNode.idx;
                var windowID = eNode.wndIdx;
                var node = eNode.node;
                var rect = eNode.rect;
                var key = node.key;
                eNode.rect = GUI.Window(windowID, rect, idx =>
                {
                    var editorNode = m_EditorNodes.Find(n => n.idx == rectIdx);
                    if (editorNode == null) return;
                    if (GUI.Button(new Rect(83, 2, 15, 15), string.Empty, skin.GetStyle("BtnNodeClose")))
                    {
                        var list = m_EditorNodes.Select(e => e as EditorNode).ToList();
                        editorNode.Destroy(list);
                        m_EditorNodes = list.Select(l => l as PRHelperEditorNode).ToList();
                        m_SelectedNodeIdx = -1;
                    }
                    if (GUI.Button(m_NodeParentPointRect, string.Empty, skin.GetStyle("BtnNodePConnector")))
                    {
                        /* if (m_StartPointRectIdx != -1)
                         {
                             if (!m_IsFromParent)
                             {
                                 Connect(rectIdx, m_StartPointRectIdx);
                             }
                             m_StartPointRectIdx = -1;
                         }
                         else
                         {
                             Disconnect(rectIdx, 0, true);
                             m_IsFromParent = true;
                             m_StartPointRectIdx = rectIdx;
                         }*/
                    }

                    var rects = editorNode.GetChildPointRects(m_NodeChildPointRect, true);
                    for (int j = 0; j < rects.Length; j++)
                    {
                        var r = rects[j];
                        if (GUI.Button(r, string.Empty, skin.GetStyle("BtnNodeCConnector")))
                        {
                            /*if (m_StartPointRectIdx != -1)
                            {
                                if (m_IsFromParent)
                                {
                                    Connect(m_StartPointRectIdx, rectIdx);
                                }
                                m_StartPointRectIdx = -1;
                            }
                            else
                            {
                                Disconnect(rectIdx, j);
                                m_IsFromParent = false;
                                m_StartPointRectIdx = rectIdx;
                            }*/
                        }
                    }
                    /*GUI.Box(new Rect(5, 20, 90, 25), node.isData ? key : node.type.ToString(), node.isData ? skin.GetStyle("BoxNodeData") : skin.GetStyle("BoxNodeMath"));*/
                    GUI.DragWindow(new Rect(0, 0, m_NodeWidthHeight.x, m_NodeWidthHeight.y));
                }, string.Empty);
            });

            /*for (int i = 0; i < m_EditorNodes.Count; i++)
            {
                var rectIdx = m_EditorNodes[i].idx;
                var windowID = m_EditorNodes[i].wndIdx;
                var node = m_EditorNodes[i].node;
                var rect = m_EditorNodes[i].rect;
                var key = node.key;

                m_EditorNodes[i].rect = GUI.Window(windowID, rect, idx =>
                {
                    var editorNode = m_EditorNodes.Find(n => n.idx == rectIdx);
                    if (editorNode == null) return;
                    if (GUI.Button(new Rect(83, 2, 15, 15), string.Empty, skin.GetStyle("BtnNodeClose")))
                    {
                        var list = m_EditorNodes.Select(e => e as EditorNode).ToList();
                        editorNode.Destroy(list);
                        m_EditorNodes = list.Select(l => l as PRHelperEditorNode).ToList();
                        m_SelectedNodeIdx = -1;
                    }
                    if (GUI.Button(m_NodeParentPointRect, string.Empty, skin.GetStyle("BtnNodePConnector")))
                    {
                        if (m_StartPointRectIdx != -1)
                        {
                            if (!m_IsFromParent)
                            {
                                Connect(rectIdx, m_StartPointRectIdx);
                            }
                            m_StartPointRectIdx = -1;
                        }
                        else
                        {
                            Disconnect(rectIdx, 0, true);
                            m_IsFromParent = true;
                            m_StartPointRectIdx = rectIdx;
                        }
                    }

                    var rects = editorNode.GetChildPointRects(m_NodeChildPointRect, true);
                    for (int j = 0; j < rects.Length; j++)
                    {
                        var r = rects[j];
                        if (GUI.Button(r, string.Empty, skin.GetStyle("BtnNodeCConnector")))
                        {
                            if (m_StartPointRectIdx != -1)
                            {
                                if (m_IsFromParent)
                                {
                                    Connect(m_StartPointRectIdx, rectIdx);
                                }
                                m_StartPointRectIdx = -1;
                            }
                            else
                            {
                                Disconnect(rectIdx, j);
                                m_IsFromParent = false;
                                m_StartPointRectIdx = rectIdx;
                            }
                        }
                    }
                    GUI.Box(new Rect(5, 20, 90, 25), node.isData ? key : node.type.ToString(), node.isData ? skin.GetStyle("BoxNodeData") : skin.GetStyle("BoxNodeMath"));
                    GUI.DragWindow(new Rect(0, 0, m_NodeWidthHeight.x, m_NodeWidthHeight.y));
                }, string.Empty);
            }*/
        }

        private void OnVMPopFuncMenu()
        {
            var currentEvent = Event.current;

            if (currentEvent.type == EventType.ContextClick)
            {
                Vector2 mousePos = currentEvent.mousePosition;
                if (m_VMWndRect.Contains(mousePos))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var funcContent in m_VMContents)
                    {
                        var str = funcContent.text;
                        menu.AddItem(funcContent, false, obj =>
                        {
                            var node = prHelper.nodes.Find(p => p.nodeType == NodeType.VM_PREvents);
                            if (node == null)
                            {
                                node = new PRHelperNode();
                                prHelper.nodes.Add(node);
                                node.nodeType = NodeType.VM_PREvents;
                            }
                            var eventType = (PREvents.EventType)Enum.Parse(typeof(PREvents.EventType), str);
                            Tools.PRHelper.PRHelper.OnEvent(node, eventType);

                            var rect = new Rect(mousePos.x, mousePos.y, m_NodeWidthHeight.x, m_NodeWidthHeight.y);
                            var idx = m_EditorNodes.Count;
                            var windowID = UnityEngine.Random.Range(10, int.MaxValue);
                            var editorNode = new PRHelperEditorNode(idx, windowID, rect, node);
                            m_EditorNodes.Add(editorNode);
                            m_SelectedNodeIdx = idx;
                            m_IsNew = true;
                        }, funcContent.ToString());
                    }
                    menu.ShowAsContext();
                    currentEvent.Use();
                }
            }
        }
    }
}