using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tool.PRHelper.Properties
{
    [Serializable]
    public class BtnBack
    {
        public Button button;
        public string prePnlName;
        public string nextPnlName;

        public void Init()
        {
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    var peek = UIManager.instance.peek;
                    if (peek == null) return;
                    Debug.Log(peek.name);
                    if (prePnlName == UIManager.instance.peek.name)
                    {
                        var nextPnl = UIManager.instance.pnlGameObjects.Find(p => p.name == nextPnlName);
                        nextPnl.SetActive(true);

                        var prePnl = UIManager.instance.pnlGameObjects.Find(p => p.name == prePnlName);
                        prePnl.SetActive(false);
                    }
                });
            }
        }

        public void Play()
        {
        }
    }
}