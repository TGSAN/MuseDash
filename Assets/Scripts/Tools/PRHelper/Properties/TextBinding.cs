using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class TextBinding
    {
        public class Binding
        {
            public Text text;
        }

        public string name;
        public string jsonPath;
        public string jsonKey;

        public void Play()
        {
        }
    }
}