using Assets.Scripts.Tool.PRHelper.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayTween))]
    public class PlayTweenDrawer : PropertyDrawer
    {
        private float height = 15;
        private float gap = 5;
        private float typeWidth = 200;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var width = rect.width;
            rect = new Rect(rect.x, rect.y, typeWidth, height);
            rect = EditorUtils.MakeEnumPropertyField("tweenType", property, rect, new GUIContent(string.Empty), gap, height);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 100;
        }
    }
}