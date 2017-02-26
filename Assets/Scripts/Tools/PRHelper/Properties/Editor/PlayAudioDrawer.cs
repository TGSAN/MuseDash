using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(PlayAudio))]
    public class PlayAudioDrawer : PropertyDrawer
    {
        private float m_AudioClipHeight = 15;
        private float m_IsPlayHeight = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = EditorUtils.MakePropertyField("audioClip", property, rect, m_Gap, m_AudioClipHeight);
            EditorUtils.MakePropertyField("isPlay", property, rect, m_Gap, m_IsPlayHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_AudioClipHeight + 2 * m_Gap + m_IsPlayHeight;
        }
    }
}