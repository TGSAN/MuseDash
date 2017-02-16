using UnityEngine;

namespace Assets.Scripts.Tools.Commons
{
    public class ResourcesLoader<T> where T : Object
    {
        public static T Load(string path)
        {
            var obj = Resources.Load(path);
            return obj as T;
        }
    }
}