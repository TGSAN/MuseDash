using System;
using System.Runtime.InteropServices;
using Assets.Scripts.Common;
using DG.Tweening;
using EasyEditor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Editor
{
    [CustomPropertyDrawer(typeof(PRHelperNode))]
    public class PRHelperNodeDrawer : PropertyDrawer
    {
        private readonly float m_Gap = 5;
        private readonly float m_KeyNameHeight = 15;
        private readonly float m_NodeTypeHeight = 15;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.BeginChangeCheck();

            rect = new Rect(rect.x, rect.y, rect.width, 0);
            var nodeTypeProperty = property.FindPropertyRelative("nodeType");
            rect = EditorUtil.MakePropertyField("nodeType", property, rect, m_Gap, m_NodeTypeHeight);
            var enumType = (PRHelperNode.NodeType)Enum.Parse(typeof(PRHelperNode.NodeType), nodeTypeProperty.enumNames[nodeTypeProperty.enumValueIndex]);
            rect = EditorUtil.MakePropertyField("key", property, rect, m_Gap, m_KeyNameHeight);
            var propertyName = StringUtils.LastAfter(enumType.ToString(), '_');
            propertyName = StringUtils.FirstToLower(propertyName);
            var showProperty = property.FindPropertyRelative(propertyName);
            if (showProperty != null)
            {
                EditorGUI.PropertyField(rect, showProperty);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var nodeTypeProperty = property.FindPropertyRelative("nodeType");
            var enumType = (PRHelperNode.NodeType)Enum.Parse(typeof(PRHelperNode.NodeType), nodeTypeProperty.enumNames[nodeTypeProperty.enumValueIndex]);

            var propertyName = StringUtils.LastAfter(enumType.ToString(), '_');
            propertyName = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
            var showProperty = property.FindPropertyRelative(propertyName);

            if (showProperty != null)
            {
                return 2 * m_Gap + m_KeyNameHeight + m_NodeTypeHeight + EditorGUI.GetPropertyHeight(showProperty);
            }
            return 2 * m_Gap + m_KeyNameHeight + m_NodeTypeHeight;
        }
    }

    [CustomPropertyDrawer(typeof(PRHelperNode.NodeType))]
    public class NodeTypeDrawer : PropertyDrawer
    {
        private readonly float m_Height = 15;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = new Rect(rect.x, rect.y, rect.width, m_Height);
            var strs = property.enumNames;
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i].Replace('_', '/');
            }
            var popupList = EditorUtil.GetGUIContentArray(strs);
            property.enumValueIndex = EditorGUI.Popup(rect, new GUIContent("Node Type"), property.enumValueIndex, popupList);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_Height;
        }
    }

    [CustomPropertyDrawer(typeof(PRHelperNode.PlayAnimation))]
    public class PlayAnimationDrawer : PropertyDrawer
    {
        private readonly float m_AnimatorHeight = 15;
        private readonly float m_StateNameHeight = 15;
        private readonly float m_SingleEventHeight = 43;
        private readonly float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtil.MakePropertyField("animator", property, rect, m_Gap, m_AnimatorHeight);
            rect = EditorUtil.MakePropertyField("stateName", property, rect, m_Gap, m_StateNameHeight);
            rect = EditorUtil.MakePropertyField("animationType", property, rect, m_Gap, m_StateNameHeight);

            var hasStart = property.FindPropertyRelative("hasStart");
            var hasPlaying = property.FindPropertyRelative("hasPlaying");
            var hasFinish = property.FindPropertyRelative("hasFinish");

            var tWidth = 200f;
            var tHeight = 20f;
            var startRect = new Rect(rect.x, rect.y + rect.height + m_Gap, tWidth, tHeight);
            var playingRect = new Rect(startRect.x + startRect.width, startRect.y, tWidth, tHeight);
            var finishRect = new Rect(playingRect.x + playingRect.width, startRect.y, tWidth, tHeight);
            hasStart.boolValue = EditorUtil.ToggleButton(hasStart.boolValue, startRect,
                new GUIContent("onStart"));
            hasPlaying.boolValue = EditorUtil.ToggleButton(hasPlaying.boolValue, playingRect,
                new GUIContent("onPlaying"));
            hasFinish.boolValue = EditorUtil.ToggleButton(hasFinish.boolValue, finishRect,
                new GUIContent("onFinish"));
            if (hasStart.boolValue)
            {
                rect = EditorUtil.MakeEventPropertyField("onAnimationStart", property, rect, m_Gap * 8, m_SingleEventHeight);
            }
            if (hasPlaying.boolValue)
            {
                rect = EditorUtil.MakeEventPropertyField("onAnimationPlaying", property, rect, m_Gap * 8, m_SingleEventHeight);
            }
            if (hasFinish.boolValue)
            {
                rect = EditorUtil.MakeEventPropertyField("onAnimationFinish", property, rect, m_Gap * 8, m_SingleEventHeight);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasStart = property.FindPropertyRelative("hasStart").boolValue;
            var hasPlaying = property.FindPropertyRelative("hasPlaying").boolValue;
            var hasFinish = property.FindPropertyRelative("hasFinish").boolValue;
            var startEventCount = EditorUtil.GetUnityEventCount(property.FindPropertyRelative("onAnimationStart"));
            startEventCount = hasStart ? (startEventCount == 0 ? 1 : startEventCount) : 0;
            var playingEventCount = EditorUtil.GetUnityEventCount(property.FindPropertyRelative("onAnimationPlaying"));
            playingEventCount = hasPlaying ? (playingEventCount == 0 ? 1 : playingEventCount) : 0;
            var finishEventCount = EditorUtil.GetUnityEventCount(property.FindPropertyRelative("onAnimationFinish"));
            finishEventCount = hasFinish ? (finishEventCount == 0 ? 1 : finishEventCount) : 0;
            var width = 40;
            var gap = 10;
            var extra = (hasStart ? width + gap : 0) + (hasPlaying ? width : 0) + (hasFinish ? width : 0);
            return 90 + (startEventCount + playingEventCount + finishEventCount) * m_SingleEventHeight + extra;
        }
    }

    [CustomPropertyDrawer(typeof(PRHelperNode.Active))]
    public class ActiveDrawer : PropertyDrawer
    {
        private float m_GOHeight = 15;
        private float m_ActiveHeight = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtil.MakePropertyField("go", property, rect, m_Gap, m_GOHeight);
            EditorUtil.MakePropertyField("isActive", property, rect, m_Gap, m_ActiveHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_GOHeight + 2 * m_Gap + m_ActiveHeight;
        }
    }

    [CustomPropertyDrawer(typeof(PRHelperNode.PlayAudio))]
    public class PlayAudioDrawer : PropertyDrawer
    {
        private float m_GOHeight = 15;
        private float m_ActiveHeight = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtil.MakePropertyField("audioClip", property, rect, m_Gap, m_GOHeight);
            EditorUtil.MakePropertyField("isActive", property, rect, m_Gap, m_ActiveHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_GOHeight + 2 * m_Gap + m_ActiveHeight;
        }
    }

    [CustomPropertyDrawer(typeof(DOTweenAnimation))]
    public class PlayTwnDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
    }

    [CustomPropertyDrawer(typeof(PRHelperNode.PREvents))]
    public class PREventsDrawer : PropertyDrawer
    {
        private readonly float m_Gap = 20;
        private readonly float m_TypeWidth = 150;
        private readonly float m_TypeHeight = 10;
        private readonly float m_BtnWidth = 15;
        private readonly float m_BtnHeight = 15;
        private readonly float m_SingleEventHeight = 43;
        private readonly float m_EventExtraHeight = 30;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = new Rect(rect.x, rect.y + m_Gap, rect.width, 0);
            var eventsProperty = property.FindPropertyRelative("events");
            for (int i = 0; i < eventsProperty.arraySize + 1; i++)
            {
                rect = MakeEventPropertyField(rect, eventsProperty, i);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var eventsProperty = property.FindPropertyRelative("events");
            var eventTypeCount = eventsProperty.arraySize;
            var eventTotalHeight = 15f;
            for (int i = 0; i < eventTypeCount; i++)
            {
                var eventProperty = eventsProperty.GetArrayElementAtIndex(i);
                var unityEventProperty = eventProperty.FindPropertyRelative("unityEvent");
                var eventCount = EditorUtil.GetUnityEventCount(unityEventProperty);
                eventCount = eventCount == 0 ? 1 : eventCount;
                var extraHeight = m_EventExtraHeight + m_TypeHeight + m_Gap / 2;
                eventTotalHeight += eventCount * m_SingleEventHeight + extraHeight;
            }
            return eventTotalHeight;
        }

        private Rect MakeEventPropertyField(Rect rect, SerializedProperty property, int idx)
        {
            if (idx < property.arraySize)
            {
                var eventProperty = property.GetArrayElementAtIndex(idx);
                var eventType = eventProperty.FindPropertyRelative("eventType");
                var enumRect = new Rect(rect.x, rect.y + rect.height, m_TypeWidth, m_TypeHeight);
                eventType.enumValueIndex = EditorGUI.Popup(enumRect, eventType.enumValueIndex, eventType.enumNames);

                var EType = (PRHelperNode.PREvents.EventType)Enum.Parse(typeof(PRHelperNode.PREvents.EventType), eventType.enumNames[eventType.enumValueIndex]);
                switch (EType)
                {
                    case PRHelperNode.PREvents.EventType.OnNGUIButtonClick:
                        {
                            var rectBtn = new Rect(rect.x + 276, rect.y, rect.width / 3, rect.height);
                            EditorUtil.MakePropertyField("NGUIButton", eventProperty, rectBtn, 0, m_BtnHeight, new GUIContent(string.Empty, "NGUI Button组件"));
                        }
                        break;
                }

                var unityEventPorperty = eventProperty.FindPropertyRelative("unityEvent");
                rect = EditorUtil.MakeEventPropertyField(unityEventPorperty, rect, m_Gap, m_SingleEventHeight, m_EventExtraHeight);
            }

            var btnAddRect = new Rect(rect.x + 180, rect.y - m_Gap, m_BtnWidth, m_BtnHeight);
            var btnRemoveRect = new Rect(btnAddRect.x + btnAddRect.width, btnAddRect.y, m_BtnWidth, m_BtnHeight);
            if (GUI.Button(btnAddRect, "+"))
            {
                property.InsertArrayElementAtIndex(idx);
            }
            if (GUI.Button(btnRemoveRect, "-"))
            {
                property.DeleteArrayElementAtIndex(idx);
            }
            return rect;
        }
    }

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
    }

    public class PRHelperWindow : EditorWindow
    {
        private SerializedObject m_SerializedObj;

        // Add menu named "My Window" to the Window menu
        [MenuItem("Window/My Window")]
        private static void Init()
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
            EditorGUILayout.PropertyField(m_SerializedObj.FindProperty("tool"));
            m_SerializedObj.ApplyModifiedProperties();
        }
    }
}