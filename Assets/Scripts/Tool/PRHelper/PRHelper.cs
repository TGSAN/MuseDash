using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common;
using DG.Tweening;
using Smart.Types;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tool.PRHelper
{
    [Serializable]
    public class PRHelperNode
    {
        public string key;
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

        public void Init()
        {
            pREvents.Init();
        }

        public void Play()
        {
            var myType = GetType();
            var nodeTypeStr = myType.GetField("nodeType").GetValue(this).ToString();
            nodeTypeStr = StringUtils.LastAfter(nodeTypeStr, '_');
            nodeTypeStr = StringUtils.FirstToLower(nodeTypeStr);
            var node = myType.GetField(nodeTypeStr).GetValue(this);
            node.GetType().GetMethod("Play").Invoke(node, null);
        }

        public enum NodeType
        {
            None,
            UI_Action_PlayAnimation,
            UI_Action_PlayTween,
            UI_Action_PlayAudio,
            UI_Action_Active,
            UI_Data_TextBinding,
            Event_PREvents,
        }

        [Serializable]
        public class Active
        {
            public GameObject go;
            public bool isActive;

            public void Play()
            {
                Debug.Log("Set Active");
                go.SetActive(isActive);
            }
        }

        [Serializable]
        public class PlayAnimation
        {
            public enum AnimtionType
            {
                Forward,
                Reverse,
                PingPong,
            }

            public enum ActionAfterFinish
            {
                KeepCurrent,
                Resume,
            }

            public Animator animator;
            public AnimtionType animationType;
            public ActionAfterFinish afterAction;
            public string stateName;
            public string preStateName;
            public bool hasStart = false, hasPlaying = false, hasFinish = false;
            public UnityEventGameObject onAnimationStart, onAnimationPlaying, onAnimationFinish;

            public void Play()
            {
                if (animator != null && !string.IsNullOrEmpty(stateName))
                {
                    animator.Play(stateName);
                }
                if (afterAction == ActionAfterFinish.Resume)
                {
                    //animator.
                }
            }
        }

        [Serializable]
        public class PlayTween
        {
            public DOTweenAnimation doTweenAnimation;

            public void Play()
            {
                Debug.Log("Play Tween");
                //doTweenAnimation.DOPlay();
            }
        }

        [Serializable]
        public class PlayAudio
        {
            public AudioClip audioClip;

            public void Play()
            {
                Debug.Log("Play Audio");
                //doTweenAnimation.DOPlay();
            }
        }

        [Serializable]
        public class PREvents
        {
            public enum EventType
            {
                OnAwake,
                OnStart,
                OnEnable,
                OnUpdate,
                OnFixedUpdate,
                OnDisable,
                OnDestroy,

                OnCollisionEnter,
                OnCollisionStay,
                OnCollisionExit,

                OnTriggerEnter,
                OnTriggerStay,
                OnTriggerExit,

                OnButtonClick,
                OnButtonHover,
                OnButtonUp,
                OnButtonDown,
                OnButton,

                OnNGUIButtonClick,
            }

            [Serializable]
            public class PREvent
            {
                public EventType eventType;
                public UnityEventGameObject unityEvent;
                public Button button;
                public UIButton NGUIButton;
                public GameObject gameObject;
            }

            public List<PREvent> events;

            public void Play()
            {
                Debug.Log("Play Event");
            }

            public void Init()
            {
                events.ForEach(e =>
                {
                    switch (e.eventType)
                    {
                        case EventType.OnNGUIButtonClick:
                            {
                                e.NGUIButton.onClick.Add(new EventDelegate(() =>
                                {
                                    e.unityEvent.Invoke(null);
                                }));
                            }
                            break;
                    }
                });
            }
        }
    }

    public class PRHelper : MonoBehaviour
    {
        public PRHelperNode[] nodes;

        public PRHelperNode this[string key]
        {
            get { return nodes.ToList().Find(n => n.key == key); }
        }

        public void Play(string key)
        {
            this[key].Play();
        }

        private void Awake()
        {
            nodes.ToList().ForEach(n => n.Init());
        }

        public void PRDebug(string s)
        {
            Debug.Log(s);
        }
    }
}