using System.Linq;
using Assets.Scripts.NewUI;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
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
            rect = EditorUtils.MakePopupField(property, "pnlName", new GUIContent("Popup Panel Name"),
                pnlNames, rect, m_Gap, m_Height);

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

            var btnNames = UIManager.instance[property.FindPropertyRelative("pnlName").stringValue].GetComponentsInChildren<Button>().ToList().Select(b => b.gameObject.name).ToArray();
            rect = EditorUtils.MakePopupField(property, "shutButtonName", new GUIContent("Shut Button"),
                btnNames, rect, m_Gap, m_Height, false, null, false, "None");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var extra1 = property.FindPropertyRelative("isFadeIn").boolValue ? 20 : 0;
            var extra2 = property.FindPropertyRelative("isFadeOut").boolValue ? 20 : 0;

            return 320 + extra1 + extra2;
        }
    }
}