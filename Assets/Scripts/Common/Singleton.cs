using UnityEngine;

namespace Assets.Scripts.Common
{
    public class Singleton<T> where T : new()
    {
        private static T m_Instance;

        /// <summary>
        /// Get singleton instance
        /// </summary>
        ///
        public static T instance
        {
            get
            {
                if (m_Instance == null) m_Instance = new T();
                return m_Instance;
            }
        }
    }

    public class SingletonScriptObject<T> : ScriptableObject where T : ScriptableObject
    {
        protected static T m_Instance;

        /// <summary>
        /// Get singleton instance
        /// </summary>
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = ScriptableObject.CreateInstance<T>();
                }
                return m_Instance;
            }
        }

        public void DestroyInstance()
        {
            ScriptableObject.Destroy(m_Instance);
            m_Instance = null;
        }
    }

    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
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
                    m_Instance = FindObjectOfType<T>();
                    if (m_Instance == null)
                    {
                        Debug.LogWarningFormat("There is no a {0} in the scene", typeof(T).ToString());
                        m_Instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
                return m_Instance;
            }
        }

        public void DestroyInstance()
        {
#if UNITY_EDITOR
            DestroyImmediate(gameObject);
#else
            Destroy(gameObject);
#endif
            m_Instance = null;
        }

        private void Awake()
        {
            if (this != instance)
            {
                Destroy(this.gameObject);
            }
        }
    }
}