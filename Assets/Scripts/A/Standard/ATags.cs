using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace A
{
    [AddComponentMenu("A/Standard/Tags")]
    public class ATags : MonoBehaviour
    {
        public ATag[] tags;

        public bool HasTag(string tag)
        {
            return tags.Any((t) => t.tag == tag);
        }
    }
}
