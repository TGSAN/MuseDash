using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.NewUI;
using Assets.Scripts.Tools.Commons;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
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

            var pnlNames = UIManager.instance.pnlNames;
            pnlNames = ArrayUtils<string>.Add(pnlNames, "None");
            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var pnlContents = EditorUtils.GetGUIContentArray(pnlNames);
            var curNameProperty = property.FindPropertyRelative("curPnlName");
            var curIdx = pnlNames.ToList().FindIndex(p => p == curNameProperty.stringValue);
            var curNameIdx = EditorGUI.Popup(rect, new GUIContent("Current Panel Name"), curIdx == -1 ? 0 : curIdx, pnlContents);
            curNameProperty.stringValue = pnlContents[curNameIdx].text;

            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var nextNameProperty = property.FindPropertyRelative("nextPnlName");
            var nextIdx = pnlNames.ToList().FindIndex(p => p == nextNameProperty.stringValue);
            var nextNameIdx = EditorGUI.Popup(rect, new GUIContent("Next Panel Name"), nextIdx == -1 ? pnlContents.Length : nextIdx, pnlContents);
            nextNameProperty.stringValue = nextNameIdx >= pnlContents.Length ? "None" : pnlContents[nextNameIdx].text;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 70;
        }
    }
}