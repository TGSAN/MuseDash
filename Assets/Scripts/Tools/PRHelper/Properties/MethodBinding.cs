using System;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class MethodBinding
    {
        public string name;
        public UnityEngine.Object sourceObj;
        public string fieldName;

        public string key;
        public string index;

        public void Play(GameObject go)
        {
            var btn = go.GetComponentsInChildren<Button>().ToList().Find(t => t.gameObject.name == name);

            var collectionBindingNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
            index = collectionBindingNode != null ? collectionBindingNode.collectionBinding.index : ReflectionUtil.Reflect(sourceObj, fieldName);

            var obj = ConstanceManager.instance[key];
            var func = obj as Func<string>;
            if (func == null)
            {
                var funcParam = obj as Func<object, string>;
                if (funcParam != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                        funcParam(index);
                    });
                }
            }
            else
            {
                btn.onClick.AddListener(() =>
                {
                    func();
                });
            }
        }
    }
}