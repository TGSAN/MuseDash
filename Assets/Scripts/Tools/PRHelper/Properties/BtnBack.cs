using System;
using Assets.Scripts.NewUI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class BtnBack
    {
        public Button button;
        public string curPnlName;
        public string nextPnlName;

        public void Init(GameObject go)
        {
            button = button ?? go.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    var top = UIManager.instance.top;
                    if (top == null) return;
                    if (curPnlName == top.name)
                    {
                        var nextPnl = UIManager.instance[nextPnlName];
                        if (nextPnl == null)
                        {
                            UIManager.instance.peek.SetActive(true);
                        }
                        else
                        {
                            nextPnl.SetActive(true);
                        }

                        var curPnl = UIManager.instance[curPnlName];
                        curPnl.SetActive(false);
                    }
                });
            }
        }

        public void Play()
        {
        }
    }
}