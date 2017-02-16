using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(TextBinding))]
    public class TextBindingDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var txts = new List<Text>();
            var txt = parent.GetComponent<Text>();
            if (txt != null) txts.Add(txt);

            var childTxts = parent.GetComponentsInChildren<Text>();
            childTxts.ToList().ForEach(txts.Add);

            rect = EditorUtils.MakePopupField(property, "name", new GUIContent("Text Name"),
                childTxts.Select(c => c.name).ToArray(), rect, m_Gap, m_Height, null, "None");
            var strs = ConfigManager.instance.configs.Select(c => c.path).ToList();
            rect = EditorUtils.MakePopupField(property, "jsonPath", new GUIContent("Json Path"),
                 strs.ToArray(), rect, m_Gap, m_Height);
            var fileName = ConfigManager.instance.configs.Find(c => c.path == property.FindPropertyRelative("jsonPath").stringValue).fileName;
            var jdata = ConfigManager.instance[fileName];
            var isArray = jdata.IsArray || jdata.Keys.Contains("0") || jdata.Keys.Contains("1");
            if (!isArray)
            {
                rect = EditorUtils.MakePopupField(property, "jsonKey", new GUIContent("Json Key"),
                 jdata.Keys.ToArray(), rect, m_Gap, m_Height);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 100;
        }
    }
}