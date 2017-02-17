using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class TextBinding
    {
        public string name;
        public SourceType type;
        public UnityEngine.Object sourceObj;

        public string path;
        public string key;

        public string fieldName;

        public string value;

        public void Play(GameObject go)
        {
            var txt = go.GetComponentsInChildren<Text>().ToList().Find(t => t.gameObject.name == name);
            txt.text = value;
        }

        public enum SourceType
        {
            Json,
            Enum,
            Script,
        }
    }
}