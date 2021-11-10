using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "A/Standard/GUID")]
    public class AGUID : ScriptableObject
    {
        public string GUID;

        public static implicit operator string(AGUID aguid) => aguid.GUID;

        private void Awake()
        {
            if (GUID == null)
                GUID = System.Guid.NewGuid().ToString();
        }

        private void OnValidate()
        {
            Awake();
        }
    }
}
