using Assets.Scripts.Tool.Commons;
using Assets.Scripts.Tool.PRHelper.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(Active))]
    public class ActiveDrawer : PropertyDrawer
    {
        private float m_GOHeight = 15;
        private float m_ActiveHeight = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtils.MakePropertyField("go", property, rect, m_Gap, m_GOHeight);
            EditorUtils.MakePropertyField("isActive", property, rect, m_Gap, m_ActiveHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_GOHeight + 2 * m_Gap + m_ActiveHeight;
        }
    }
}