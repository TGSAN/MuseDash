using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Common
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private List<TKey> m_Keys;

        [SerializeField]
        private List<TValue> m_Values;

        public void OnBeforeSerialize()
        {
            m_Keys = new List<TKey>(Count);
            m_Values = new List<TValue>(Count);
            foreach (var kvp in this)
            {
                m_Keys.Add(kvp.Key);
                m_Values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
            int count = Mathf.Min(m_Keys.Count, m_Values.Count);
            for (int i = 0; i < count; ++i)
            {
                var key = m_Keys[i];
                if (!ContainsKey(key))
                {
                    Add(m_Keys[i], m_Values[i]);
                }
            }
        }
    }

    [System.Serializable]
    public class StringDictionary : SerializableDictionary<string, string>
    {
    }
}