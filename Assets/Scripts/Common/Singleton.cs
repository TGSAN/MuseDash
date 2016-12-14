using FormulaBase;
using System.Collections;
using UnityEngine;

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
                    Debug.LogFormat("There is no a {0} in the scene", typeof(T).ToString());
                    //m_Instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    protected void Awake()
    {
        if (this != instance)
        {
            Destroy(this.gameObject);
        }
    }
}