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
        public string reflectName;

        public string key;
        public string index;

        public void Play(GameObject go)
        {
            var btn = go.GetComponentsInChildren<Button>().ToList().Find(t => t.gameObject.name == name);

            var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
            index = cBNode != null ? cBNode.collectionBinding.index : (sourceObj != null ? ReflectionUtil.Reflect(sourceObj, reflectName) : index);

            var obj = CallbackManager.instance[key];
            var func = obj as Action;
            if (func == null)
            {
                var funcParam = obj as Action<object>;
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