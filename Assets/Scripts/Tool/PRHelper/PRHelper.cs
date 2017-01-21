using System.Linq;
using EasyEditor;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper
{
    public class PRHelper : MonoBehaviour
    {
        [HideInInspector]
        public bool isNewNode = true;

        [Inspector(group = "Create New Node")]
        public string newNodeName;

        [Inspector(group = "Create New Node")]
        public void CreateNode()
        {
            if (string.IsNullOrEmpty(newNodeName)) return;
            PRNodeMaker.MakerPRNode(newNodeName);
        }

        [Inspector(group = "Node Function")]
        public PRHelperNode[] nodes;

        public PRHelperNode this[string key]
        {
            get { return nodes.ToList().Find(n => n.key == key); }
        }

        private void Awake()
        {
            nodes.ToList().ForEach(n => n.Init());
        }
    }
}