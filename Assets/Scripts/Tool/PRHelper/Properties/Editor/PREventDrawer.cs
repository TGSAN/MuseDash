using System;
using Assets.Scripts.Tool.PRHelper.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PREvents))]
    public class PREventsDrawer : PropertyDrawer
    {
        private readonly float m_Gap = 20;
        private readonly float m_TypeWidth = 150;
        private readonly float m_TypeHeight = 10;
        private readonly float m_AddButtonHeight = 15;
        private readonly float m_BtnWidth = 15;
        private readonly float m_BtnHeight = 15;
        private readonly float m_SingleEventHeight = 43;
        private readonly float m_EventExtraHeight = 25;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = new Rect(rect.x, rect.y + m_Gap, rect.width, 0);
            var eventsProperty = property.FindPropertyRelative("events");
            for (var i = 0; i < eventsProperty.arraySize; i++)
            {
                rect = MakeEventPropertyField(rect, eventsProperty, i);
            }
            if (eventsProperty.arraySize == 0)
            {
                rect = new Rect(rect.x, rect.y, rect.width, m_AddButtonHeight);
                if (GUI.Button(rect, "Add Event"))
                {
                    property.FindPropertyRelative("events").InsertArrayElementAtIndex(0);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var eventsProperty = property.FindPropertyRelative("events");
            var eventTypeCount = eventsProperty.arraySize;
            var eventTotalHeight = 25f;
            for (int i = 0; i < eventTypeCount; i++)
            {
                var eventProperty = eventsProperty.GetArrayElementAtIndex(i);
                var unityEventProperty = eventProperty.FindPropertyRelative("unityEvent");
                var eventCount = EditorUtils.GetUnityEventCount(unityEventProperty);
                eventCount = eventCount == 0 ? 1 : eventCount;
                var extraHeight = m_EventExtraHeight + m_TypeHeight + m_Gap / 2;
                eventTotalHeight += eventCount * m_SingleEventHeight + extraHeight;
            }
            return eventTotalHeight;
        }

        private Rect MakeEventPropertyField(Rect rect, SerializedProperty property, int idx)
        {
            var eventProperty = property.GetArrayElementAtIndex(idx);
            var eventType = eventProperty.FindPropertyRelative("eventType");
            var enumRect = new Rect(rect.x, rect.y + rect.height, m_TypeWidth, m_TypeHeight);
            eventType.enumValueIndex = EditorGUI.Popup(enumRect, eventType.enumValueIndex, eventType.enumNames);

            var EType = (PREvents.EventType)Enum.Parse(typeof(PREvents.EventType), eventType.enumNames[eventType.enumValueIndex]);
            switch (EType)
            {
            }

            var unityEventPorperty = eventProperty.FindPropertyRelative("unityEvent");
            rect = EditorUtils.MakeEventPropertyField(unityEventPorperty, rect, m_Gap, m_SingleEventHeight, m_EventExtraHeight);

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
}