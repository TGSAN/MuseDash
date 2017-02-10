using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Tools.PRHelper.Properties;
using EasyEditor;
using Smart.Types;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper
{
    public class PRHelper : MonoBehaviour
    {
        [HideInInspector]
        public bool isNewNode = true;

        [Inspector(@group = "Create New Node")]
        public string newNodeName;

        [Inspector(@group = "Node Function")]
        public List<PRHelperNode> nodes = new List<PRHelperNode>();

        public PRHelperNode this[string key]
        {
            get { return nodes.Find(n => n.key == key); }
        }

        private void Awake()
        {
            nodes.ForEach(n => n.Init(gameObject));
            OnEventInvoke(PREvents.EventType.OnAwake);
        }

        public void Play(string key)
        {
            this[key].Play();
        }

        #region static Func

        public static UnityEventObject OnEvent(PRHelperNode node, PREvents.EventType eventType)
        {
            var prEvent = node.pREvents.events.Find(e => e.eventType == eventType);
            var unityEvent = new UnityEventObject();
            if (prEvent == null)
            {
                prEvent = new PREvents.PREvent(eventType, unityEvent);
                node.pREvents.events.Add(prEvent);
            }
            else
            {
                unityEvent = prEvent.unityEvent;
            }
            return unityEvent;
        }

        public static UnityEventObject OnEvent(GameObject go, PREvents.EventType eventType)
        {
            var prHelper = go.GetComponent<PRHelper>() ?? go.AddComponent<PRHelper>();
            var node = prHelper.nodes.Find(n => n.nodeType == NodeType.VM_PREvents);
            if (node == null)
            {
                node = new PRHelperNode();
                node.pREvents = new PREvents();
                node.nodeType = NodeType.VM_PREvents;
                node.isActive = true;
                prHelper.nodes.Add(node);
            }
            return OnEvent(node, eventType);
        }

        #endregion static Func

        #region Event Func

        private void OnEventInvoke(PREvents.EventType eventType, object args = null)
        {
            nodes.ForEach(n =>
            {
                n.pREvents.events.Where(e => e.eventType == eventType).ToList().ForEach(e => e.unityEvent.Invoke(args as GameObject));
            });
        }

        private void Start()
        {
            OnEventInvoke(PREvents.EventType.OnStart);
        }

        private void OnEnable()
        {
            OnEventInvoke(PREvents.EventType.OnEnable);
        }

        private void OnDisable()
        {
            OnEventInvoke(PREvents.EventType.OnDisable);
        }

        private void Update()
        {
            OnEventInvoke(PREvents.EventType.OnUpdate);
        }

        private void FixedUpdate()
        {
            OnEventInvoke(PREvents.EventType.OnFixedUpdate);
        }

        private void OnDestroy()
        {
            OnEventInvoke(PREvents.EventType.OnDestroy);
        }

        private void OnTriggerEnter(Collider col)
        {
            OnEventInvoke(PREvents.EventType.OnTriggerEnter, col);
        }

        private void OnTriggerStay(Collider col)
        {
            OnEventInvoke(PREvents.EventType.OnTriggerStay, col);
        }

        private void OnTriggerExit(Collider col)
        {
            OnEventInvoke(PREvents.EventType.OnTriggerExit, col);
        }

        private void OnCollisionEnter(Collision col)
        {
            OnEventInvoke(PREvents.EventType.OnCollisionEnter, col);
        }

        private void OnCollisionStay(Collision col)
        {
            OnEventInvoke(PREvents.EventType.OnCollisionStay, col);
        }

        private void OnCollisionExit(Collision col)
        {
            OnEventInvoke(PREvents.EventType.OnCollisionExit, col);
        }

        #endregion Event Func
    }
}