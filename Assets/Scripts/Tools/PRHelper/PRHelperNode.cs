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

        public BtnBack btnBack;

        public Active active;

        public PlayPopup playPopup;

        public PREvents pREvents = new PREvents();

        public TextBinding textBinding;
        public ImageBinding imageBinding;
        public MethodBinding methodBinding;
        public ObjectBinding objectBinding;
        public AudioBinding audioBinding;
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
            if (btnBack != null) btnBack.Init(go);
        }

        private void VMInit(GameObject go)
        {
            if (pREvents == null) pREvents = new PREvents();
            pREvents.Init(go);

            if (go.GetComponent<Button>() != null)
            {
                PRHelper.OnEvent(go, PREvents.EventType.OnButtonClick).AddListener(obj =>
                {
                    Play(go);
                });
            }
        }

        private void ModelInit(GameObject go)
        {
            switch (nodeType)
            {
                case NodeType.Model_AudioBinding:
                case NodeType.Model_CollectionBinding:
                case NodeType.Model_ImageBinding:
                case NodeType.Model_ObjectBinding:
                case NodeType.Model_MethodBinding:
                case NodeType.Model_TextBinding:
                    PRHelper.OnEvent(go, PREvents.EventType.OnStart).AddListener(obj =>
                    {
                        Play(go);
                    });
                    break;
            }
        }
    }
}