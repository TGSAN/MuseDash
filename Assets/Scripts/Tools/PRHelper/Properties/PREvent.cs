using System;
using System.Collections.Generic;
using Smart.Types;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
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
        }

        [Serializable]
        public class PREvent
        {
            public EventType eventType;
            public UnityEventObject unityEvent;
            public Button button;
            public GameObject gameObject;

            public PREvent(EventType type, UnityEventObject e, GameObject go = null, Button btn = null)
            {
                eventType = type;
                unityEvent = e;
                gameObject = go;
                button = btn;
            }
        }

        public List<PREvent> events = new List<PREvent>();

        public void Play()
        {
            Debug.Log("Play Event");
        }

        public void Init(GameObject go)
        {
            foreach (var e in events)
            {
                e.gameObject = e.gameObject ?? go;
                e.button = e.button ?? go.GetComponent<Button>();
                switch (e.eventType)
                {
                    case EventType.OnButtonClick:
                        {
                            var theEvent = e;
                            if (e.button != null)
                            {
                                e.button.onClick.AddListener(() =>
                                {
                                    theEvent.unityEvent.Invoke(null);
                                });
                            }
                        }
                        break;
                }
            }
        }
    }
}