using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.FormulaTreeManager.Editor
{
    [Serializable]
    public class FormulaEditorNode : EditorNode
    {
        public FormulaNode node;

        public FormulaEditorNode(int id, int wid, Rect r, FormulaNode n)
        {
            idx = id;
            wndIdx = wid;
            rect = r;
            node = n;
        }

        public override void AddChild(EditorNode childNode, List<EditorNode> allEditorNodes, params object[] args)
        {
            base.AddChild(childNode, allEditorNodes);
            var n = childNode as FormulaEditorNode;
            if (n != null) node.AddChild(n.node.GetUid());
        }

        public override void RemoveChild(EditorNode childNode, List<EditorNode> allEditorNodes = null, params object[] args)
        {
            base.RemoveChild(childNode, allEditorNodes);
            var n = childNode as FormulaEditorNode;
            if (n != null) node.RemvoeChild(n.node.GetUid());
        }

        public override void SetParent(EditorNode parentNode, List<EditorNode> allEditorNodes)
        {
            base.SetParent(parentNode, allEditorNodes);
            var n = parentNode as FormulaEditorNode;
            node.SetParent(n != null ? n.node.GetUid() : string.Empty);
        }

        public override void Destroy(List<EditorNode> allEditorNodes)
        {
            base.Destroy(allEditorNodes);
            if (node.parent != null)
            {
                node.parent.RemvoeChild(node.GetUid());
            }
            node.childs.ForEach(c => node.RemvoeChild(c.GetUid()));
            node.RemoveSelfFromList();
        }
    }

	public class SingtonEditor<T> : EditorWindow where T : EditorWindow
	{
		private static T m_Instance;

		/// <summary>
		/// Get singleton instance
		/// </summary>
		public static T instance
		{
			get
			{
				if (m_Instance == null)
				{
					m_Instance = EditorWindow.GetWindow<T>();
					if (m_Instance == null)
					{
						Debug.LogWarningFormat("There is no a {0} in the editor", typeof(T).ToString());
					}
				}
				return m_Instance;
			}
		}
	}
    public class FormulaTreeEditor : SingtonEditor<FormulaTreeEditor>
    {
        #region layout config

        private static float m_WindowHeight = 1000;
        private static float m_WindowWidth = 1100;
        private static float m_PathWindowHeight = 15;
        private static float m_LeftWidth = 300;
        private static Vector2 m_NodeEditWindowWH = new Vector2(200, 200);
        private static Vector2 m_NodeWidthHeight = new Vector2(100, 65);
        private Rect m_PathWindowRect = new Rect(0, 0, m_LeftWidth, m_PathWindowHeight);
        private Rect m_DataWindowRect = new Rect(0, 70, m_LeftWidth, m_WindowHeight);
        private Rect m_TreeWindowRect = new Rect(m_LeftWidth, 0, m_WindowWidth - m_LeftWidth, m_WindowHeight);
        private Rect m_TypeNameWindowRect = new Rect(100, 150, 150, 100);
        private Rect m_NodeEditWindowRect = new Rect(m_LeftWidth, 20, m_NodeEditWindowWH.x, m_NodeEditWindowWH.y);
        private Rect m_NodeParentPointRect = new Rect(m_NodeWidthHeight.x / 2 - 9, 3, 17, 17);
        private Rect m_NodeChildPointRect = new Rect(m_NodeWidthHeight.x / 2 - 45, 48, 13, 13);
        private int m_PathWindowIdx = 1;
        private int m_DataWindowIdx = 2;
        private int m_TypeNameWindowIdx = 3;
        private int m_NodeEditWindowIdx = 4;

        private Vector2 m_DynamicWindowOffset;
        private SerializedObject m_FormulaTreeObj;
        private bool m_NewTypeName = false;
        private string m_CurTypeName = string.Empty;
        private int m_SelectedTypeIdx = 0;
        private bool m_CreateFlag = false;
        private int m_SelectedNodeIdx = -1;
        private int m_StartPointRectIdx = -1;
        private bool m_IsNew = false;
        private bool m_IsFromParent = false;

        [SerializeField]
        private List<string> m_TypeNames = new List<string>();

        [SerializeField]
        private List<FormulaEditorNode> m_EditorNodes = new List<FormulaEditorNode>();

        private GUIContent[] m_FuncContents = new[]
        {
             new GUIContent("Data/Constance"),
            new GUIContent("Data/Variable"),

            new GUIContent("Maths/Add"),
            new GUIContent("Maths/Minus"),
            new GUIContent("Maths/Mul"),
            new GUIContent("Maths/Divid"),
            new GUIContent("Maths/Pow"),
            new GUIContent("Maths/Sqrt"),
            new GUIContent("Maths/Log"),
        };

        private static GUISkin m_Skin;

        public static GUISkin skin
        {
            get
            {
                if (m_Skin == null)
                {
                    if (EditorGUIUtility.isProSkin)
                    {
                        m_Skin =
                            AssetDatabase.LoadAssetAtPath(StringCommons.PRFormulaTreeEditorSkinPathInAssets, typeof(GUISkin))
                                as GUISkin;
                    }
                }
                return m_Skin;
            }
        }

        public List<EditorNode> editorNodes
        {
            get { return m_EditorNodes.Select(n => (EditorNode)n).ToList(); }
        }

        #endregion layout config

        public FormulaEditorNode curEditorNode
        {
            get { return m_EditorNodes.Find(n => n.idx == m_SelectedNodeIdx); }
        }

        public Vector2 curStartPointPos
        {
            get
            {
                if (m_IsFromParent)
                {
                    return ParentPointPos(m_StartPointRectIdx, 0);
                }
                return ChildPointPos(m_StartPointRectIdx);
            }
        }

        [MenuItem(StringCommons.FormulaTreeMenuItem)]
        private static void Init()
        {
            var window = EditorWindow.GetWindow(typeof(FormulaTreeEditor), false, "Formula Tree");
            window.Show();
        }

        public static void CreateFormulaData()
        {
            var formulaTree = FormulaTree.instance;
            var path = EditorPrefs.GetString("FormulaTree Path");
            StringCommons.FormulaTreePathInResources = string.IsNullOrEmpty(path) ? StringCommons.FormulaTreePathInResources : path;
            AssetDatabase.CreateAsset(formulaTree, StringCommons.ResourcesPathInAssets + StringCommons.FormulaTreePathInResources + ".asset");
        }

        private void OnEnable()
        {
            var res = Resources.Load<FormulaTree>(StringCommons.FormulaTreePathInResources);
            if (res != null)
            {
                m_FormulaTreeObj = new SerializedObject(res);
            }
        }

        private void OnGUI()
        {
            GUIEvent();
            InitSkin();
            Repaint();

            BeginWindows();
            //Path Edit Window
            OnPathEditWindow();
            //Left Data Edit Window
            OnDataEditWindow();
            //Mid Data Edit Window
            OnTreeWindow();
            //Node Edite Window
            OnNodeEditorWindow();
            //Left Click Edit Window
            OnPopFuncMenu();
            EndWindows();

            OnCurvesDraw();
            OnCurrentCurveDraw();
        }

        private void OnCurvesDraw()
        {
            foreach (var editorNode in m_EditorNodes)
            {
                if (editorNode.parentCNodes.Count > 0)
                {
                    var p = editorNode.parentCNodes[0];
                    if (p != null)
                    {
                        EditorUtils.DrawCurve(ParentPointPos(p.childIdx, 0), ChildPointPos(p.parentIdx, p.pos));
                    }
                }
            }
        }

        private void OnCurrentCurveDraw()
        {
            if (m_StartPointRectIdx != -1)
            {
                EditorUtils.DrawCurve(curStartPointPos,
                    Event.current.mousePosition);
            }
        }

        private void InitSkin()
        {
            GUI.skin = skin;
        }

        private void GUIEvent()
        {
            var curEvent = Event.current;
            if (!m_TreeWindowRect.Contains(curEvent.mousePosition)) return;
            if (curEvent.type == EventType.mouseDown)
            {
                var node = m_EditorNodes.Find(n => n.rect.Contains(curEvent.mousePosition));
                if (node != null)
                {
                    m_SelectedNodeIdx = node.idx;
                }
                else
                {
                    if (!m_NodeEditWindowRect.Contains(curEvent.mousePosition))
                    {
                        m_SelectedNodeIdx = -1;
                    }
                }

                var isInBtn = false;
                foreach (var editorNode in m_EditorNodes)
                {
                    var childPointRects = editorNode.GetChildPointRects(m_NodeChildPointRect);
                    var parentPointRects = editorNode.GetParentPointRects(m_NodeParentPointRect);
                    isInBtn = childPointRects.ToList().Exists(r => r.Contains(curEvent.mousePosition)) || parentPointRects[0].Contains(curEvent.mousePosition);
                    if (isInBtn) break;
                }
                if (m_StartPointRectIdx != -1 && !isInBtn)
                {
                    m_StartPointRectIdx = -1;
                }
            }
            if (m_SelectedNodeIdx == -1)
            {
                GUI.UnfocusWindow();
            }
            else
            {
                var curWindowID = m_EditorNodes.Find(n => n.idx == m_SelectedNodeIdx).wndIdx;
                if (curEvent.type == EventType.mouseDown || m_IsNew)
                {
                    GUI.FocusWindow(!m_NodeEditWindowRect.Contains(curEvent.mousePosition)
                        ? curWindowID
                        : m_NodeEditWindowIdx);
                    m_IsNew = false;
                }
            }
        }

        private void OnPathEditWindow()
        {
            m_PathWindowRect = GUILayout.Window(m_PathWindowIdx, m_PathWindowRect, idx =>
            {
                StringCommons.FormulaTreePathInResources = GUILayout.TextField(StringCommons.FormulaTreePathInResources);
                if (GUILayout.Button("Setup"))
                {
                    CreateFormulaData();
                    OnEnable();
                }
                EditorPrefs.SetString("FormulaTree Path", StringCommons.FormulaTreePathInResources);
            }, "Path Edit");
        }

        private void OnDataEditWindow()
        {
            if (m_FormulaTreeObj == null) return;
            m_DataWindowRect = GUILayout.Window(m_DataWindowIdx, m_DataWindowRect, idx =>
            {
                m_FormulaTreeObj.Update();
                var pairs = m_FormulaTreeObj.FindProperty("formulaObjs").FindPropertyRelative("pairs");
                if (GUILayout.Button("New Group"))
                {
                    m_NewTypeName = true;
                }
                if (m_CreateFlag)
                {
                    pairs.InsertArrayElementAtIndex(pairs.arraySize);
                    var newPair = pairs.GetArrayElementAtIndex(pairs.arraySize - 1);
                    newPair.FindPropertyRelative("type").stringValue = m_CurTypeName;
                    m_CreateFlag = false;
                }

                m_SelectedTypeIdx = GUILayout.SelectionGrid(m_SelectedTypeIdx, m_TypeNames.ToArray(), m_TypeNames.Count);
                GUILayout.BeginVertical();
                m_DynamicWindowOffset = GUILayout.BeginScrollView(m_DynamicWindowOffset, false, true);

                for (int i = 0; i < pairs.arraySize; i++)
                {
                    var pair = pairs.GetArrayElementAtIndex(i);
                    var key = pair.FindPropertyRelative("key");
                    var value = pair.FindPropertyRelative("value");
                    var typeName = pair.FindPropertyRelative("type").stringValue;
                    if (!m_TypeNames.Contains(typeName))
                    {
                        m_TypeNames.Add(typeName);
                        m_SelectedTypeIdx = m_TypeNames.Count - 1;
                    }
                    GUILayout.BeginHorizontal();

                    if (typeName == m_TypeNames[m_SelectedTypeIdx])
                    {
                        GUILayout.Label("Key");
                        EditorGUILayout.PropertyField(key, GUIContent.none);
                        GUILayout.Label("Value");
                        EditorGUILayout.PropertyField(value.FindPropertyRelative("value"), GUIContent.none);
                        if (GUILayout.Button("+", EditorStyles.miniButtonMid))
                        {
                            pairs.InsertArrayElementAtIndex(i);
                            var newPair = pairs.GetArrayElementAtIndex(i + 1);
                            newPair.FindPropertyRelative("type").stringValue = typeName;
                        }
                        if (GUILayout.Button("-", EditorStyles.miniButtonRight))
                        {
                            pairs.DeleteArrayElementAtIndex(i);
                            var isContains = false;
                            for (int j = 0; j < pairs.arraySize; j++)
                            {
                                var type = pairs.GetArrayElementAtIndex(j).FindPropertyRelative("type").stringValue;
                                if (type == typeName)
                                {
                                    isContains = true;
                                    break;
                                }
                            }
                            if (!isContains)
                            {
                                m_TypeNames.Remove(typeName);
                                if (m_TypeNames.Count > 0)
                                {
                                    m_SelectedTypeIdx = m_TypeNames.Count - 1;
                                }
                            }
                        }
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                m_FormulaTreeObj.ApplyModifiedProperties();
            }, "Dynamic Params");
            if (m_NewTypeName)
            {
                m_TypeNameWindowRect = GUILayout.Window(m_TypeNameWindowIdx, m_TypeNameWindowRect, idx =>
                {
                    GUI.DragWindow(new Rect(0, 0, 100, 10));
                    m_CurTypeName = GUILayout.TextField(m_CurTypeName);
                    GUILayout.Space(5);
                    if (GUILayout.Button("Finish"))
                    {
                        m_TypeNames.Add(m_CurTypeName);
                        m_SelectedTypeIdx = m_TypeNames.Count - 1;
                        m_NewTypeName = false;
                        m_CreateFlag = true;
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Cancell"))
                    {
                        m_NewTypeName = false;
                    }
                }, "Add Type Name");
            }
        }

        private void OnTreeWindow()
        {
            GUI.Box(new Rect(300, 0, 800, 20), "Tree Graph");

            for (int i = 0; i < m_EditorNodes.Count; i++)
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
                        m_EditorNodes = list.Select(l => l as FormulaEditorNode).ToList();
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
            }
        }

        private void OnNodeEditorWindow()
        {
            if (m_SelectedNodeIdx == -1) return;
            var editorNode = curEditorNode;

            m_NodeEditWindowRect = GUI.Window(m_NodeEditWindowIdx, m_NodeEditWindowRect, idx =>
            {
                editorNode.node.key = GUILayout.TextField(editorNode.node.key);
                editorNode.node.type = (FormulaNodeType)EditorGUILayout.EnumPopup("type", editorNode.node.type);
                if (editorNode.node.value != null)
                {
                    GUILayout.Label(editorNode.node.value.value.ToString());
                }

                /*m_FormulaTreeObj.Update();
                var head = m_FormulaTreeObj.FindProperty("head").FindPropertyRelative("type");
                EditorGUILayout.PropertyField(head, GUIContent.none);
                m_FormulaTreeObj.ApplyModifiedProperties();*/
            }, "Node Edit");
        }

        private void OnPopFuncMenu()
        {
            var currentEvent = Event.current;
            if (currentEvent.type == EventType.ContextClick)
            {
                Vector2 mousePos = currentEvent.mousePosition;
                if (m_TreeWindowRect.Contains(mousePos))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var funcContent in m_FuncContents)
                    {
                        var str = funcContent.text;
                        menu.AddItem(funcContent, false, obj =>
                        {
                            var formulaNode = new FormulaNode();
                            formulaNode.AddSelfToList();
                            if (str.Contains("Maths/"))
                            {
                                formulaNode.type = (FormulaNodeType)Enum.Parse(typeof(FormulaNodeType),
                                    str.Replace("Maths/", string.Empty));
                            }
                            else
                            {
                                formulaNode.type = (FormulaNodeType)Enum.Parse(typeof(FormulaNodeType),
                                    str.Replace("Data/", string.Empty));
                                formulaNode.key = "0";
                            }

                            var rect = new Rect(mousePos.x, mousePos.y, m_NodeWidthHeight.x, m_NodeWidthHeight.y);
                            var idx = m_EditorNodes.Count;
                            var windowID = UnityEngine.Random.Range(10, int.MaxValue);
                            var editorNode = new FormulaEditorNode(idx, windowID, rect, formulaNode);
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

        private void Connect(int childIdx, int parentIdx)
        {
            if (childIdx == parentIdx) return;
            var childEditorNode = m_EditorNodes.Find(n => n.idx == childIdx);
            var parentEditorNode = m_EditorNodes.Find(n => n.idx == parentIdx);
            var nodes = editorNodes;
            var allParentInParent = parentEditorNode.AllParentIdx(nodes);
            var allParentInChild = childEditorNode.AllParentIdx(nodes);
            if (allParentInParent.Exists(idx => idx == childIdx)) return;
            if (allParentInChild.Exists(idx => idx == parentIdx)) return;
            childEditorNode.SetParent(parentEditorNode, nodes);
            parentEditorNode.AddChild(childEditorNode, nodes);
        }

        private void Disconnect(int idx, int pos, bool isParent = false)
        {
            var editorNode = m_EditorNodes.Find(n => n.idx == idx);
            EditorNode parentNode = null;
            if (!isParent)
            {
                if (editorNode.childCNodes.Count <= pos) return;
                var childIdx = editorNode.childCNodes[pos].childIdx;
                parentNode = editorNode;
                editorNode = m_EditorNodes.Find(n => n.idx == childIdx);
            }
            else
            {
                parentNode = m_EditorNodes.Find(n => n.idx == editorNode.parentCNodes[pos].parentIdx);
            }

            editorNode.SetParent(null, editorNodes);
            parentNode.RemoveChild(editorNode, editorNodes);
        }

        private Rect GetParentPointRect(int idx, int pos)
        {
            var editorNode = m_EditorNodes.Find(n => n.idx == idx);
            return editorNode.GetParentPointRects(m_NodeParentPointRect)[pos];
        }

        private Vector2 ParentPointPos(int idx, int pos)
        {
            var editorNode = m_EditorNodes.Find(n => n.idx == idx);
            return editorNode.ParentPointPos(m_NodeParentPointRect, pos);
        }

        private Vector2 ChildPointPos(int idx, int pos)
        {
            var eidorNode = m_EditorNodes.Find(n => n.idx == idx);
            return eidorNode.ChildPointPos(m_NodeChildPointRect, pos);
        }

        private Vector2 ChildPointPos(int idx)
        {
            var eidorNode = m_EditorNodes.Find(n => n.idx == idx);
            return eidorNode.ChildPointPos(m_NodeChildPointRect);
        }
    }
}