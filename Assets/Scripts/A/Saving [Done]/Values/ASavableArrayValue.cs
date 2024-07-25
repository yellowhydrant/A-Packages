using UnityEngine;

namespace A.Saving.Values
{
    public abstract class ASavableArrayValue<T> : ASavableObject
    {
        public T[] runtimeValue = System.Array.Empty<T>();
        [field: SerializeField]
        public T[] DefaultValue { get; private set; } = System.Array.Empty<T>();

        public override string DataSlotSubDirectory => ASavableValueConstants.SaveSlotSubDirectory;

        public static implicit operator T[](ASavableArrayValue<T> value) => value.runtimeValue;

        internal ASavableArrayValue()
        {

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
            Debug.Log(json);
            runtimeValue = JsonUtility.FromJson<Wrapper>(json).Items;
        }

        [System.Serializable]
        class Wrapper
        {
            public T[] Items;
        }
    }
}
