using System;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.PRHelper.Properties;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper
{
    [Serializable]
    public class PRHelperNode
    {
        public string key = "fucker";
        public NodeType nodeType;

        public PlayAnimation playAnimation;

        public PlayTween playTween;

        public PlayAudio playAudio;

        public BtnBack btnBack;

        public Active active;

        public PlayPopup playPopup;

        public PREvents pREvents = new PREvents();

        public TextBinding textBinding;

        public void Play(GameObject go)
        {
            var myType = GetType();
            var nodeTypeStr = myType.GetField("nodeType").GetValue(this).ToString();
            nodeTypeStr = StringUtils.LastAfter(nodeTypeStr, '_');
            nodeTypeStr = StringUtils.FirstToLower(nodeTypeStr);
            var node = myType.GetField(nodeTypeStr).GetValue(this);
            node.GetType().GetMethod("Play").Invoke(node, new object[] { go });
        }

        public void Init(GameObject go)
        {
            PRHelper.OnEvent(go, PREvents.EventType.OnButtonClick).AddListener(obj =>
            {
                Play(go);
            });

            if (textBinding != null && !string.IsNullOrEmpty(textBinding.name))
            {
                PRHelper.OnEvent(go, PREvents.EventType.OnUpdate).AddListener(obj =>
                {
                    Play(go);
                });
            }

            if (pREvents == null) pREvents = new PREvents();
            pREvents.Init(go);

            if (btnBack != null) btnBack.Init(go);
        }
    }
}