using System.Linq;
using Assets.Scripts.Tool.PRHelper.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayPopup))]
    public class PlayPopupDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var pnlNames = UIManager.instance.pnlNames;
            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var pnlContents = EditorUtils.GetGUIContentArray(pnlNames);
            var nameProperty = property.FindPropertyRelative("pnlName");
            var idx = pnlNames.ToList().FindIndex(p => p == nameProperty.stringValue);
            var nameIdx = EditorGUI.Popup(rect, new GUIContent("Popup Panel Name"), idx == -1 ? 0 : idx, pnlContents);
            nameProperty.stringValue = pnlContents[nameIdx].text;
            rect = EditorUtils.MakePropertyField("bkgColor", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("speed", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("distance", property, rect, m_Gap, m_Height);
            //rect = EditorUtils.MakePropertyField("moveInDt", property, rect, m_Gap, m_Height);
            //rect = EditorUtils.MakePropertyField("moveOutDt", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("moveInEase", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("moveOutEase", property, rect, m_Gap, m_Height);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 165;
        }
    }
}