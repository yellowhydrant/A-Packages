using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable]
    public abstract class ASavableSimpleValue<T> : ASavableObject
    {
        public T runtimeValue;
        [field: SerializeField]
        public T DefaultValue { get; private set; }

        public override string DataSlotSubDirectory => ASavableValueConstants.SaveSlotSubDirectory;

        public static implicit operator T(ASavableSimpleValue<T> value) => value.runtimeValue;

        internal ASavableSimpleValue()
        {

        }

        public override void ResetValue()
        {
            runtimeValue = DefaultValue;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(new Wrapper { Item = runtimeValue});
        }

        public override void FromJson(string json)
        {
            runtimeValue = JsonUtility.FromJson<Wrapper>(json).Item;
        }

        [System.Serializable]
        class Wrapper
        {
            public T Item;
        }
    }
}
