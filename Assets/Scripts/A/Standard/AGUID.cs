using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "A/Standard/GUID")]
    public class AGUID : ScriptableObject
    {
        [field: SerializeField]
        public string GUID { get; private set; } = System.Guid.NewGuid().ToString();

        public static implicit operator string(AGUID aguid) => aguid.GUID;

        //public void Init(string guid)
        //{
        //    if (GUID == null)
        //        GUID = guid;
        //}

        //private void Awake()
        //{
        //    if (GUID == null)
        //        GUID = System.Guid.NewGuid().ToString();
        //}
    }
}
