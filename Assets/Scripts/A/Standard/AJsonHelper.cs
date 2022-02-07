using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace A
{
    public static class AJsonHelper
    {
        public static IEnumerable<T> FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(IEnumerable<T> enumerable, bool prettyPrint = false)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = enumerable.ToArray();
            return JsonUtility.ToJson(wrapper);
        }

        [Serializable]
        class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
