#if UNITY_EDITOR
using System;
using System.Collections;
using System.Linq;
using A.Extensions;
using UnityEditor;
using UnityEngine;

namespace A.Editor
{
    //[CustomEditor(typeof(AEnumDictionary<,>), true)]
    public abstract class AEnumObjectDictionaryEditor<TEnum, TValue> : UnityEditor.Editor where TEnum : Enum where TValue : UnityEngine.Object
    {
        bool[] foldoutStates;
        TEnum[] enums;
        AEnumObjectDictionary<TEnum, TValue> dict;

        private void OnEnable()
        {
            dict = target as AEnumObjectDictionary<TEnum, TValue>;
            var names = typeof(TEnum).GetEnumNames();
            enums = new TEnum[names.Length];
            for (int i = 0; i < enums.Length; i++)
                enums[i] = (TEnum)Enum.Parse(typeof(TEnum), names[i]);

            if (dict.values == null || dict.values.Count != enums.Length)
            {
                dict.values = new MyBox.MyDictionary<TEnum, TValue>();
                for (int i = 0; i < enums.Length; i++)
                    dict.values.Add(enums[i], dict.defaultObject);
            }
            foldoutStates = Enumerable.Repeat(true, enums.Length).ToArray();
        }

        public override void OnInspectorGUI()
        {
            for (int i = 0; i < enums.Length; i++)
            {
                var state = EditorGUILayout.Foldout(foldoutStates[i], enums[i].AddWhiteSpace());
                foldoutStates[i] = state;
                if (state)
                {
                    dict.values[enums[i]] = (TValue)EditorGUILayout.ObjectField(dict.values[enums[i]], typeof(TValue), false);
                }
            }
        }
    }
}
#endif
