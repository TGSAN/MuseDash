using System;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.PRHelper.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper
{
    [Serializable]
    public class PRHelperNode
    {
        public string key = string.Empty;
        public NodeType nodeType;

        public PlayAnimation playAnimation;
        public PlayTween playTween;
        public PlayAudio playAudio;
        public PlayPopup playPopup;
        public PlayActive playActive;
        public PlayScrollRect playScrollRect;
        public BtnBack btnBack;

        public PREvents pREvents = new PREvents();

        public PREvents.EventType eventType;

        public TextBinding textBinding;
        public ImageBinding imageBinding;
        public MethodBinding methodBinding;
        public ObjectBinding objectBinding;
        public AudioBinding audioBinding;
        public SpineBinding spineBinding;
        public CollectionBinding collectionBinding;

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
            ModelInit(go);
            VMInit(go);
            ViewInit(go);
        }

        private void ViewInit(GameObject go)
        {
        }

        private void VMInit(GameObject go)
        {
            if (pREvents == null) pREvents = new PREvents();
            pREvents.Init(go);

            if (go.GetComponent<Button>() != null && eventType == PREvents.EventType.None)
            {
                eventType = PREvents.EventType.OnButtonClick; ;
            }

            PRHelper.OnEvent(go, eventType).AddListener(obj =>
            {
                Play(go);
            });
        }

        private void ModelInit(GameObject go)
        {
        }
    }

    [Serializable]
    public class ReflectObject
    {
        public UnityEngine.Object sourceObj;
        public string reflectName;
    }
}