using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Tool.PRHelper.Properties;
using GameLogic;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper
{
    public class UIManager : SingletonMonoBehaviour<UIManager>
    {
        public List<GameObject> pnlGameObjects = new List<GameObject>();
        private Stack<GameObject> m_InactiveStack = new Stack<GameObject>();

        public GameObject peek
        {
            get
            {
                if (m_InactiveStack.Count > 0)
                {
                    return m_InactiveStack.Peek();
                }
                return null;
            }
        }

        public string[] pnlNames
        {
            get { return pnlGameObjects.Select(p => p.name).ToArray(); }
        }

        public void Push(GameObject pnlGO)
        {
            m_InactiveStack.Push(pnlGO);
        }

        public void Pop(GameObject pnlGO)
        {
            if (m_InactiveStack.Count == 0) return;
            var list = new List<GameObject>();
            var thePeek = m_InactiveStack.Pop();
            while (thePeek != null)
            {
                list.Add(thePeek);
                thePeek = m_InactiveStack.Pop();
            }
            if (list.Contains(pnlGO))
            {
                list.Remove(pnlGO);
            }
            m_InactiveStack = new Stack<GameObject>();
            list.Reverse();
            foreach (var gameObject in list)
            {
                m_InactiveStack.Push(gameObject);
            }
            list.ForEach(g => Debug.Log(g.name));
        }

        public void Awake()
        {
            foreach (var pnlGameObject in pnlGameObjects)
            {
                PRHelper.OnEvent(pnlGameObject, PREvents.EventType.OnDisable).AddListener(Push);
                PRHelper.OnEvent(pnlGameObject, PREvents.EventType.OnEnable).AddListener(Pop);
            }
        }
    }
}