using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayScrollRect))]
    public class PlayScrollRectDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtils.MakeAllPropertyField(property, typeof(PlayScrollRect), rect, m_Gap, m_Height);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 280;
        }
    }
}