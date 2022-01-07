using System.Runtime.Serialization;
using UnityEngine;

namespace A.Saving
{
    [System.Serializable]
    public abstract class ASavableObject : ScriptableObject
    {
        [field: SerializeField, HideInInspector]
        public string guid { get; private set; }
        public virtual string SaveSlotSubDirectory { get; }

        protected virtual void Awake()
        {
            if (guid == null)
                guid = System.Guid.NewGuid().ToString();
        }

        public abstract void ResetValue();
        public abstract string ToJson();
        public abstract void FromJson(string json);
    }
}
