using System;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using Spine.Unity;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class SpineBinding
    {
        public string name;
        public string animationName;

        public ReflectObject reflectObj;

        public string path;
        public string key;
        public string index;
        private static Material m_SpineMtrl;

        public void Play(GameObject go)
        {
            var transforms = go.GetComponentsInChildren<Transform>().ToList();
            transforms.Add(go.transform);
            var gameObject = transforms.Find(t => t.name == name).gameObject;
            var skeletonGraphic = gameObject.AddComponent<SkeletonGraphic>();

            var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
            index = cBNode != null ? cBNode.collectionBinding.index : (reflectObj.sourceObj != null ? ReflectionUtil.Reflect(reflectObj) : index);

            var resourcePath = ConfigManager.instance.GetConfigStringValue(ConfigManager.instance.GetFileName(path),
                int.Parse(index), key);
            var skeletonAsset = ResourcesLoader.Load<SkeletonDataAsset>(resourcePath);
            if (m_SpineMtrl == null)
            {
                m_SpineMtrl = ResourcesLoader.Load<Material>("materials/skeleton_graphic_default");
            }
            skeletonGraphic.material = m_SpineMtrl;
            skeletonGraphic.startingLoop = true;
            skeletonGraphic.skeletonDataAsset = skeletonAsset;
            skeletonGraphic.startingAnimation = animationName;
            skeletonGraphic.Initialize(true);
        }
    }
}