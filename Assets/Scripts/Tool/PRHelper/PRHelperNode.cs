using System;
using Assets.Scripts.Common;
using Assets.Scripts.Tool.PRHelper.Properties;

namespace Assets.Scripts.Tool.PRHelper
{
    [Serializable]
    public class PRHelperNode
    {
        public string key;
        public bool isActive;
        public NodeType nodeType;

        //UI_Action_PlayAnimtion
        public PlayAnimation playAnimation;

        //UI_Action_PlayTween
        public PlayTween playTween;

        //UI_Action_PlayAudio
        public PlayAudio playAudio;

        //UI_Action_Active
        public Active active;

        public PREvents pREvents;

        public void Play()
        {
            var myType = GetType();
            var nodeTypeStr = myType.GetField("nodeType").GetValue(this).ToString();
            nodeTypeStr = StringUtils.LastAfter(nodeTypeStr, '_');
            nodeTypeStr = StringUtils.FirstToLower(nodeTypeStr);
            var node = myType.GetField(nodeTypeStr).GetValue(this);
            node.GetType().GetMethod("Play").Invoke(node, null);
        }

        public void Init()
        {
            pREvents.Init();
        }
    }
}