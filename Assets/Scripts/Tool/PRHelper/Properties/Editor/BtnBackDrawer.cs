using System.Linq;
using Assets.Scripts.Tool.PRHelper.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(BtnBack))]
    public class BtnBackDrawer : PropertyDrawer
    {
        private float m_Gap = 5;
        private float m_Height = 15;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var btnProperty = property.FindPropertyRelative("button");
            rect = EditorUtils.MakePropertyField(btnProperty, rect, m_Gap, m_Height);

            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var pnlNames = EditorUtils.GetGUIContentArray(UIManager.instance.pnlNames);
            var preNameProperty = property.FindPropertyRelative("prePnlName");
            var preIdx = UIManager.instance.pnlNames.ToList().FindIndex(p => p == preNameProperty.stringValue);
            var preNameIdx = EditorGUI.Popup(rect, new GUIContent("Previous Panel Name"), preIdx == -1 ? 0 : preIdx, pnlNames);
            preNameProperty.stringValue = pnlNames[preNameIdx].text;

            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var nextNameProperty = property.FindPropertyRelative("nextPnlName");
            var nextIdx = UIManager.instance.pnlNames.ToList().FindIndex(p => p == nextNameProperty.stringValue);
            var nextNameIdx = EditorGUI.Popup(rect, new GUIContent("Next Panel Name"), nextIdx == -1 ? 0 : nextIdx, pnlNames);
            nextNameProperty.stringValue = pnlNames[nextNameIdx].text;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 70;
        }
    }
}