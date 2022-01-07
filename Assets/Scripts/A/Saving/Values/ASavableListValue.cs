using System.Collections.Generic;
using UnityEngine;

namespace A.Saving.Values
{
    public abstract class ASavableListValue<T> : ASavableObject
    {
        public List<T> runtimeValue = new List<T>();
        [field: SerializeField]
        public List<T> DefaultValue { get; private set; }

        public override string SaveSlotSubDirectory => ASavableValueConstants.SaveSlotSubDirectory;

        public static implicit operator List<T>(ASavableListValue<T> value) => value.runtimeValue;

        internal ASavableListValue()
        {

        }

        protected override void Awake()
        {
            base.Awake();
            if (DefaultValue == null)
                DefaultValue = new List<T>();
        }

        public override void ResetValue()
        {
            runtimeValue = DefaultValue;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(new Wrapper { Items = runtimeValue });
        }

        public override void FromJson(string json)
        {
            runtimeValue = JsonUtility.FromJson<Wrapper>(json).Items;
        }

        [System.Serializable]
        class Wrapper
        {
            public List<T> Items;
        }
    }
}
