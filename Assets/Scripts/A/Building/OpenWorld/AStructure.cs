using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("A/Building/OpenWorld/Structure")]
public class AStructure : MonoBehaviour
{
    public Vector3 scaleFactor;
    public Vector3 offset;

    public Collider structureCollider;
    [SerializeField] BoxCollider placementCollider;
    [SerializeField] BoxCollider[] contactColliders;

    public bool CanPlace(LayerMask mask)
    {
        var overlaps = Physics.OverlapBox(placementCollider.bounds.center, placementCollider.bounds.extents, placementCollider.transform.rotation, mask).Where((c) => !c.isTrigger && c != structureCollider).ToArray();
        return !overlaps.Any();
    }

    public bool IsConnected()
    {
        for (int i = 0; i < contactColliders.Length; i++)
        {
            if (Physics.OverlapBox(contactColliders[i].bounds.center, contactColliders[i].bounds.extents, contactColliders[i].transform.rotation).Where((c) => !c.isTrigger && c != structureCollider).Any())
                return true;
        }
        return false;
    }
}
