using System.Linq;
using Assets.Scripts.NewUI;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.PRHelper.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayPopup))]
    public class PlayPopupDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var pnlNames = UIManager.instance.pnlNames;
            if (pnlNames.Length <= 0)
            {
                return;
            }
            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var pnlContents = EditorUtils.GetGUIContentArray(pnlNames);
            var nameProperty = property.FindPropertyRelative("pnlName");
            var idx = pnlNames.ToList().FindIndex(p => p == nameProperty.stringValue);
            idx = idx == -1 ? 0 : idx;
            var nameIdx = EditorGUI.Popup(rect, new GUIContent("Popup Panel Name"), idx, pnlContents);
            if (nameIdx < pnlNames.Length)
            {
                nameProperty.stringValue = pnlNames[nameIdx];
            }

            rect = EditorUtils.MakeLabelField(new GUIContent("In"), rect, m_Gap, m_Height, PRHelperEditor.skin.GetStyle("Bold"));
            rect = EditorUtils.MakePropertyField("inDistance", property, rect, m_Gap, m_Height, new GUIContent("Distance"));
            rect = EditorUtils.MakePropertyField("inTime", property, rect, m_Gap, m_Height, new GUIContent("Time"));
            rect = EditorUtils.MakePropertyField("moveInEase", property, rect, m_Gap, m_Height, new GUIContent("Ease"));
            rect = EditorUtils.MakePropertyField("isFadeIn", property, rect, m_Gap, m_Height, new GUIContent("Fade"));
            if (property.FindPropertyRelative("isFadeIn").boolValue)
            {
                rect = EditorUtils.MakePropertyField("fadeInEase", property, rect, m_Gap, m_Height, new GUIContent("Ease"));
            }

            rect = EditorUtils.MakeLabelField(new GUIContent("Out"), rect, m_Gap, m_Height, PRHelperEditor.skin.GetStyle("Bold"));
            rect = EditorUtils.MakePropertyField("outDistance", property, rect, m_Gap, m_Height, new GUIContent("Distance"));
            rect = EditorUtils.MakePropertyField("outTime", property, rect, m_Gap, m_Height, new GUIContent("Time"));
            rect = EditorUtils.MakePropertyField("moveOutEase", property, rect, m_Gap, m_Height, new GUIContent("Ease"));
            rect = EditorUtils.MakePropertyField("isFadeOut", property, rect, m_Gap, m_Height, new GUIContent("Fade"));
            if (property.FindPropertyRelative("isFadeOut").boolValue)
            {
                rect = EditorUtils.MakePropertyField("fadeOutEase", property, rect, m_Gap, m_Height, new GUIContent("Ease"));
            }

            rect = EditorUtils.MakeLabelField(new GUIContent("Mask"), rect, m_Gap, m_Height, PRHelperEditor.skin.GetStyle("Bold"));
            rect = EditorUtils.MakePropertyField("color", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakeLabelField(new GUIContent("Blur", "Invalid"), rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("shut", property, rect, m_Gap, m_Height);

            rect = new Rect(rect.x, rect.y + m_Height + m_Gap, rect.width, rect.height);
            var btnNamePpt = property.FindPropertyRelative("shutButtonName");
            var btnNames = UIManager.instance[nameProperty.stringValue].GetComponentsInChildren<Button>().ToList().Select(b => b.gameObject.name).ToArray();
            var btnContents = EditorUtils.GetGUIContentArray(btnNames);
            var btnIdx = btnNames.ToList().FindIndex(s => s == btnNamePpt.stringValue);
            btnIdx = btnIdx == -1 ? 0 : btnIdx;
            btnIdx = EditorGUI.Popup(rect, new GUIContent("Shut Button"), btnIdx, btnContents);
            if (btnIdx < btnNames.Length)
            {
                btnNamePpt.stringValue = btnNames[btnIdx];
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var extra1 = property.FindPropertyRelative("isFadeIn").boolValue ? 20 : 0;
            var extra2 = property.FindPropertyRelative("isFadeOut").boolValue ? 20 : 0;

            return 320 + extra1 + extra2;
        }
    }
}