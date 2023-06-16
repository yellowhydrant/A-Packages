using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Vector2/Vector2 Array")]
    public class Vector2Array : ASavableArrayValue<Vector2> { }
}
