using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Component = UnityEngine.Component;
using Object = UnityEngine.Object;

namespace com.ootii.Helpers
{
    /// <summary>
    /// Used to help manage interfaces. In this case, we're using it to help find
    /// monobehaviors that implement interfaces
    /// </summary>
    public static class InterfaceHelper
    {
        /// <summary>
        /// Hold all the types that implement the interface
        /// </summary>
        private static Dictionary<Type, Type[]> mInterfaceTypes;

        /// <summary>
        /// Constructor for class
        /// </summary>
        static InterfaceHelper()
        {
            mInterfaceTypes = new Dictionary<Type, Type[]>();
        }

        /// <summary>
        /// Grab all the component instances that derive from the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetComponents<T>()
        {
            List<T> lInterfaces = new List<T>();

            Object[] lComponents = GameObject.FindObjectsOfType(typeof(MonoBehaviour));
            for (int i = 0; i < lComponents.Length; i++)
            {
                if (lComponents[i] is T)
                {
                    T lComponent = (T)((object)lComponents[i]);
                    lInterfaces.Add(lComponent);
                }
            }

            return lInterfaces.ToArray();
        }

        /// <summary>
        /// Grab all the component instances that derive from the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetComponents<T>(GameObject rGameObject)
        {
            List<T> lInterfaces = new List<T>();

            Component[] lComponents = rGameObject.GetComponents(typeof(MonoBehaviour));
            for (int i = 0; i < lComponents.Length; i++)
            {
                if (lComponents[i] is T)
                {
                    T lComponent = (T)((object)lComponents[i]);
                    lInterfaces.Add(lComponent);
                }
            }

            return lInterfaces.ToArray();
        }

        /// <summary>
        /// Grab all the component instances that derive from the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponent<T>(GameObject rGameObject)
        {
            Type lInterfaceType = typeof(T);

            Type[] lTypes = GetInterfaceTypes(lInterfaceType);
            if (lTypes == null || lTypes.Length == 0) { return default(T); }

            for (int i = 0; i < lTypes.Length; i++)
            {
                Type lType = lTypes[i];

                // Only grab components
                if (lType.IsSubclassOf(typeof(Component)))
                {
                    object lComponent = rGameObject.GetComponent(lType) as object;
                    if (lComponent != null) { return (T)lComponent; }
                }
            }

            // Grab the distinct set of objects
            return default(T);
        }

        /// <summary>
        /// Grab all the component instances that derive from the interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] FindComponentsOfType<T>()
        {
            Type lInterfaceType = typeof(T);

            Type[] lTypes = GetInterfaceTypes(lInterfaceType);
            if (lTypes == null || lTypes.Length == 0) { return null; }

            List<T> lInstances = new List<T>();
            for (int i = 0; i < lTypes.Length; i++)
            {
                Type lType = lTypes[i];

                // Only grab components
                if (lType.IsSubclassOf(typeof(Component)))
                {
                    lInstances.AddRange(Component.FindObjectsOfType(lType).Cast<T>());
                }
            }

            // Grab the distinct set of objects
            return lInstances.Distinct().ToArray();
        }

        /// <summary>
        /// Grab all the types that implement the interface
        /// </summary>
        /// <param name="rInterface"></param>
        /// <returns></returns>
        public static Type[] GetInterfaceTypes(Type rInterface)
        {
            if (!rInterface.IsInterface) { return null; }

            if (!mInterfaceTypes.ContainsKey(rInterface))
            {
                Type[] lTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => rInterface.IsAssignableFrom(p) && p != rInterface).ToArray();
                mInterfaceTypes.Add(rInterface, lTypes);
            }

            return mInterfaceTypes[rInterface];
        }
    }
}