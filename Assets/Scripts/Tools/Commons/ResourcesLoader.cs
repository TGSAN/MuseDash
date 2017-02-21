using UnityEngine;

namespace Assets.Scripts.Tools.Commons
{
    public class ResourcesLoader
    {
        public static T Load<T>(string path) where T : Object
        {
            var obj = Resources.Load<T>(path);
            return obj;
        }

        public static void Unload(Object obj)
        {
            Resources.UnloadAsset(obj);
        }
    }
}