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
        [SerializeField]
        private List<GameObject> m_PnlGameObjects = new List<GameObject>();

        private Stack<GameObject> m_InactiveStack = new Stack<GameObject>();

        public GameObject top
        {
            get
            {
                if (m_PnlGameObjects.Count == 0) return null;
                var pnls = new List<GameObject>(m_PnlGameObjects);
                pnls.RemoveAll(p => p.name == "PnlMenu");
                pnls.Sort((l, r) =>
                {
                    if (l.activeSelf && !r.activeSelf)
                    {
                        return -1;
                    }

                    if (!l.activeSelf && r.activeSelf)
                    {
                        return 1;
                    }

                    if (!l.activeSelf && !r.activeSelf)
                    {
                        return 0;
                    }

                    if (l.transform.parent != r.transform.parent)
                    {
                        return l.transform.parent.GetSiblingIndex() - r.transform.parent.GetSiblingIndex();
                    }
                    return l.transform.GetSiblingIndex() - r.transform.GetSiblingIndex();
                });
                return pnls[0];
            }
        }

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
            get { return m_PnlGameObjects.Select(p => p.name).ToArray(); }
        }

        public GameObject this[string n]
        {
            get
            {
                return m_PnlGameObjects.Find(p => p.name == n);
            }
        }

        public void Push(GameObject pnlGO)
        {
            m_InactiveStack.Push(pnlGO);
        }

        public void Pop(GameObject pnlGO)
        {
            if (m_InactiveStack.Count == 0) return;
            var list = new List<GameObject>();
            while (m_InactiveStack.Count > 0)
            {
                list.Add(m_InactiveStack.Pop());
            }
            if (list.Contains(pnlGO))
            {
                list.Remove(pnlGO);
            }
            m_InactiveStack = new Stack<GameObject>();
            list.Reverse();
            foreach (var go in list)
            {
                m_InactiveStack.Push(go);
            }
        }

        public void Awake()
        {
            foreach (var pnlGameObject in m_PnlGameObjects)
            {
                var go = pnlGameObject;
                PRHelper.OnEvent(pnlGameObject, PREvents.EventType.OnDisable).AddListener(g =>
                {
                    Push(go);
                });
                PRHelper.OnEvent(pnlGameObject, PREvents.EventType.OnEnable).AddListener(g =>
                {
                    Pop(go);
                });
            }
        }
    }
}