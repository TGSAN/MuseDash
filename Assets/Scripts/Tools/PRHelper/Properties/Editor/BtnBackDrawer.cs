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
            rect = EditorUtils.MakePopupField(property, "curPnlName", new GUIContent("Current Panel Name"),
                pnlNames, rect, m_Gap, m_Height, false, null, false, "None");
            rect = EditorUtils.MakePopupField(property, "nextPnlName", new GUIContent("Next Panel Name"),
               pnlNames, rect, m_Gap, m_Height, false, null, false, "None");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 70;
        }
    }
}