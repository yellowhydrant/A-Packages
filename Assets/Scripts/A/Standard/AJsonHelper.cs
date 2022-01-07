using System;
using System.Linq;
using UnityEngine;

namespace A
{
    public static class AJsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        class Wrapper<T>
        {
            public T[] Items;
        }

        public static bool IsCollectionType(Type type)
        {
            return type != typeof(string) && type.GetInterfaces().Any(s => s.Namespace == "System.Collections.Generic" || s.Namespace == "System.Collections" || s.IsArray);
        }
    }
}
