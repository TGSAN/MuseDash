using UnityEngine;

namespace Assets.Scripts.Tools.Commons
{
    public class ResourcesLoader
    {
        public static T Load<T>(string path) where T : Object
        {
            var obj = Resources.Load(path);
            return obj as T;
        }
    }
}