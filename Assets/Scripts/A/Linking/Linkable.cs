using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Linking
{
    public class Linkable : MonoBehaviour, ILinkable
    {
        public ILinkable.Link[] links { get => links_; set => links_ = value; }
        public ILinkable.Link[] links_;
        public Vector2 groupPosition { get => groupPosition_; set => groupPosition_ = value; }
        [SerializeField, HideInInspector] Vector2 groupPosition_;
    }
}
