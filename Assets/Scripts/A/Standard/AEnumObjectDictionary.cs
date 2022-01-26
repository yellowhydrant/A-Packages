using System;
using UnityEngine;
using MyBox;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace A
{
    public abstract class AEnumObjectDictionary<TEnum, TValue> : ScriptableObject where TEnum : Enum where TValue : UnityEngine.Object
    {
        public TValue defaultObject;
        public MyDictionary<TEnum, TValue> values = new MyDictionary<TEnum, TValue>();
    }
}
