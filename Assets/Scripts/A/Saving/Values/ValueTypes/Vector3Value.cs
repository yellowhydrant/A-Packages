using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Vector3/Vector3 Value")]
    public class Vector3Value : ASavableSimpleValue<Vector3> { }
}
