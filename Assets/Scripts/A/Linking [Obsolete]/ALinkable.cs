using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Linking
{
    [AddComponentMenu(AConstants.ComponentMenuRoot + "/" + ALinkingConstants.ComponentMenuRoot + "/" + "Linkable")]
    public class ALinkable : MonoBehaviour, ILinkable
    {
        [field: SerializeField] public ILinkable.Link[] links { get; set; } = System.Array.Empty<ILinkable.Link>();
        [field: SerializeField] public Vector2 groupPosition { get; set; }
    }
}
