using System;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayActive))]
    public class PlayActiveDrawer : PropertyDrawer
    {
        private float m_GOHeight = 15;
        private float m_ActiveHeight = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorUtils.MakeAllPropertyField(property, typeof(PlayActive), rect, m_Gap, m_GOHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_GOHeight + 2 * m_Gap + m_ActiveHeight;
        }
    }
}