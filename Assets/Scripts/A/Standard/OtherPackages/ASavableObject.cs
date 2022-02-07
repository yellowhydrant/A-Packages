using System.Runtime.Serialization;
using A.Extensions;
using UnityEngine;

namespace A.Saving
{
    [System.Serializable]
    public abstract class ASavableObject : ScriptableObject
    {
        [field: SerializeField, HideInInspector]
        public string guid { get; } = System.Guid.NewGuid().ToString();
        public virtual string DataSlotSubDirectory { get; }

        //[ContextMenu("Fix Guid", true)]
        //bool IsGuidBroken() => !guid.IsValidGuid();

        //[ContextMenu("Fix Guid", false)]
        //protected void FixGuid()
        //{
        //    guid = System.Guid.NewGuid().ToString();
        //}

        public abstract void ResetValue();
        public abstract string ToJson();
        public abstract void FromJson(string json);
    }
}
