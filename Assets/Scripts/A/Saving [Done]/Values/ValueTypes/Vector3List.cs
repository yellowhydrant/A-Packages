using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Vector3/Vector3 List")]
    public class Vector3List : ASavableListValue<Vector3> { }
}
