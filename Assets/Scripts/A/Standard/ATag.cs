using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "A/Standard/Tag", fileName = "New Tag")]
    public class ATag : ScriptableObject
    {
        public string tag;

        public static implicit operator string(ATag tag) => tag.tag;
    }
}
