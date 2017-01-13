using System;
using System.Collections.Generic;
using System.Linq;
using Smart.Extensions;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class UIButtonBack : MonoBehaviour
    {
        [Serializable]
        public class ActionPair
        {
            public string prePnlName;
            public string aimPnlName;
            public string playAnimation;
        }

        public ActionPair[] actions;
        private UIButton m_Btn;
        private List<UIPanel> m_Pnls;

        public void Awake()
        {
            m_Btn = GetComponent<UIButton>();
			m_Pnls = new List<UIPanel> ();
			m_Btn.onClick.Insert(0, new EventDelegate(() =>
            {
				SortPnls();
				PlayAction();
            }));
        }

        private void SortPnls()
        {
			var allUIBasePhase = UISceneHelper.Instance.widgets.Values.ToList ();
			foreach (var item in allUIBasePhase) {
				var pnl = item.gameObject.GetComponent<UIPanel> ();
				if (pnl != null && pnl.gameObject.name != "PnlMainMenu") {
					m_Pnls.Add (pnl);
				}
			}
            m_Pnls.Sort((pL, pR) =>
            {
				if (!pL.gameObject.activeInHierarchy)
                {
						if (!pR.gameObject.activeInHierarchy) {
							return 0;
						}
						return 1;
                }
				if (!pR.gameObject.activeInHierarchy)
                {
						if (!pL.gameObject.activeInHierarchy) {
							return 0;
						}
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
					if (!string.IsNullOrEmpty(actionPair.aimPnlName))
                    {
						nextPnl = m_Pnls.ToList().Find(p => p.gameObject.name == actionPair.aimPnlName);
                    }
                    nextPnl.gameObject.SetActive(true);
                    var onShowScript = nextPnl.gameObject.GetComponent<UIPhaseBase>();
                    if (onShowScript != null)
                    {
                        onShowScript.OnShow();
                    }

					if (!string.IsNullOrEmpty(actionPair.playAnimation))
                    {
                        var animator = m_Btn.gameObject.GetComponent<Animator>();
                        if (animator != null)
                        {
							animator.Play(actionPair.playAnimation);
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