using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class TextBinding
    {
        public string name;
        public SourceType type;

        public string jsonPath;
        public UnityEngine.Object jsonSourceObj;
        public string jsonKey;

        public string value;

        public void Play()
        {
        }

        public void Init()
        {
        }

        public enum SourceType
        {
            Json,
            Enum,
            Script,
        }
    }
}