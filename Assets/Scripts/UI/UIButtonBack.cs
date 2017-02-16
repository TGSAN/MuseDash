using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIButtonBack : MonoBehaviour
    {
        [Serializable]
        public class ActionPair
        {
            public string prePnlName;
            public string nextPnlName;
            public string animName;
        }

        public ActionPair[] actions;
        private UIButton m_Btn;
        private List<UIPanel> m_Pnls;

        public void Awake()
        {
            m_Btn = GetComponent<UIButton>();
            m_Pnls = new List<UIPanel>((UIPanel[])UnityEngine.Object.FindObjectsOfTypeAll(typeof(UIPanel)));
            m_Pnls.RemoveAll(p => p.gameObject.name == "PnlBattle");
            m_Btn.onClick.Add(new EventDelegate(() =>
            {
                DOTweenUtils.Delay(() =>
                {
                    SortPnls();
                    PlayAction();
                }, Time.deltaTime);
            }));
        }

        private void SortPnls()
        {
            m_Pnls.Sort((pL, pR) =>
            {
                if (!pL.gameObject.activeSelf)
                {
                    return 1;
                }
                if (!pR.gameObject.activeSelf)
                {
                    return -1;
                }
                return pR.depth - pL.depth;
            });
        }

        private void PlayAction()
        {
            var pnlGO = m_Pnls[0].gameObject;
            foreach (var actionPair in actions)
            {
                if (pnlGO.name == actionPair.prePnlName)
                {
                    pnlGO.SetActive(false);

                    var nextPnl = UIPhaseBase.lastestPnl;
                    if (!string.IsNullOrEmpty(actionPair.nextPnlName))
                    {
                        nextPnl = m_Pnls.ToList().Find(p => p.gameObject.name == actionPair.nextPnlName);
                    }
                    nextPnl.gameObject.SetActive(true);
                    var onShowScript = nextPnl.gameObject.GetComponent<UIPhaseBase>();
                    if (onShowScript != null)
                    {
                        onShowScript.OnShow();
                    }

                    if (!string.IsNullOrEmpty(actionPair.animName))
                    {
                        var animator = m_Btn.gameObject.GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.Play(actionPair.animName);
                        }
                        else
                        {
                            Debug.LogWarning("No Animator On" + m_Btn.gameObject.name);
                        }
                    }
                }
            }
        }
    }
}