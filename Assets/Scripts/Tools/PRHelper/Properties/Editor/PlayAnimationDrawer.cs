using Assets.Scripts.Tools.Commons;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayAnimation))]
    public class PlayAnimationDrawer : PropertyDrawer
    {
        private readonly float m_AnimatorHeight = 15;
        private readonly float m_StateNameHeight = 15;
        private readonly float m_SingleEventHeight = 43;
        private readonly float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtils.MakePropertyField("animator", property, rect, m_Gap, m_AnimatorHeight);
            rect = EditorUtils.MakePropertyField("stateName", property, rect, m_Gap, m_StateNameHeight);
            rect = EditorUtils.MakePropertyField("animationType", property, rect, m_Gap, m_StateNameHeight);

            var hasStart = property.FindPropertyRelative("hasStart");
            var hasPlaying = property.FindPropertyRelative("hasPlaying");
            var hasFinish = property.FindPropertyRelative("hasFinish");

            var tWidth = 200f;
            var tHeight = 20f;
            var startRect = new Rect(rect.x, rect.y + rect.height + m_Gap, tWidth, tHeight);
            var playingRect = new Rect(startRect.x + startRect.width, startRect.y, tWidth, tHeight);
            var finishRect = new Rect(playingRect.x + playingRect.width, startRect.y, tWidth, tHeight);
            hasStart.boolValue = EditorUtils.ToggleButton(hasStart.boolValue, startRect,
                new GUIContent("onStart"));
            hasPlaying.boolValue = EditorUtils.ToggleButton(hasPlaying.boolValue, playingRect,
                new GUIContent("onPlaying"));
            hasFinish.boolValue = EditorUtils.ToggleButton(hasFinish.boolValue, finishRect,
                new GUIContent("onFinish"));
            if (hasStart.boolValue)
            {
                rect = EditorUtils.MakeEventPropertyField("onAnimationStart", property, rect, m_Gap * 8, m_SingleEventHeight);
            }
            if (hasPlaying.boolValue)
            {
                rect = EditorUtils.MakeEventPropertyField("onAnimationPlaying", property, rect, m_Gap * 8, m_SingleEventHeight);
            }
            if (hasFinish.boolValue)
            {
                rect = EditorUtils.MakeEventPropertyField("onAnimationFinish", property, rect, m_Gap * 8, m_SingleEventHeight);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasStart = property.FindPropertyRelative("hasStart").boolValue;
            var hasPlaying = property.FindPropertyRelative("hasPlaying").boolValue;
            var hasFinish = property.FindPropertyRelative("hasFinish").boolValue;
            var startEventCount = EditorUtils.GetUnityEventCount(property.FindPropertyRelative("onAnimationStart"));
            startEventCount = hasStart ? (startEventCount == 0 ? 1 : startEventCount) : 0;
            var playingEventCount = EditorUtils.GetUnityEventCount(property.FindPropertyRelative("onAnimationPlaying"));
            playingEventCount = hasPlaying ? (playingEventCount == 0 ? 1 : playingEventCount) : 0;
            var finishEventCount = EditorUtils.GetUnityEventCount(property.FindPropertyRelative("onAnimationFinish"));
            finishEventCount = hasFinish ? (finishEventCount == 0 ? 1 : finishEventCount) : 0;
            var width = 40;
            var gap = 10;
            var extra = (hasStart ? width + gap : 0) + (hasPlaying ? width : 0) + (hasFinish ? width : 0);
            return 90 + (startEventCount + playingEventCount + finishEventCount) * m_SingleEventHeight + extra;
        }
    }
}