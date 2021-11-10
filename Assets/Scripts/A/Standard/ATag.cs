using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = "A/Standard/Tag", fileName = "New Tag")]
    public class ATag : ScriptableObject
    {
        public string tag;
    }
}
