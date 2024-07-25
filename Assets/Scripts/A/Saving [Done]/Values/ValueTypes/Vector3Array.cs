using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Vector3/Vector3 Array")]
    public class Vector3Array : ASavableArrayValue<Vector3> { }
}
