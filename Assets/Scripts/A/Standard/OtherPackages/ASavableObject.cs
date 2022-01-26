using System.Runtime.Serialization;
using A.Extensions;
using UnityEngine;

namespace A.Saving
{
    [System.Serializable]
    public abstract class ASavableObject : ScriptableObject
    {
        [field: SerializeField, HideInInspector]
        public string guid { get; private set; }
        public virtual string SaveSlotSubDirectory { get; }

        [ContextMenu("Fix Guid", true)]
        bool IsGuidNull() => !guid.IsValidGuid();

        [ContextMenu("Fix Guid", false)]
        protected virtual void Awake()
        {
            if (!guid.IsValidGuid())
                guid = System.Guid.NewGuid().ToString();
        }

        public abstract void ResetValue();
        public abstract string ToJson();
        public abstract void FromJson(string json);
    }
}
