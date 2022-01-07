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
        HashSet<string> hashedTags;

        private void Awake()
        {
            hashedTags = new HashSet<string>(tags.Cast<string>());
        }

        public bool HasTag(string tag)
        {
            return hashedTags.Contains(tag);
        }
    }
}
