using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Common.Editor
{
    public class SingtonEditor<T> : EditorWindow where T : EditorWindow
    {
        private static T m_Instance;

        /// <summary>
        /// Get singleton instance
        /// </summary>
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = EditorWindow.GetWindow<T>();
                    if (m_Instance == null)
                    {
                        Debug.LogWarningFormat("There is no a {0} in the editor", typeof(T).ToString());
                    }
                }
                return m_Instance;
            }
        }
    }
}