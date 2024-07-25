using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Vector2/Vector2 Value")]
    public class Vector2Value : ASavableSimpleValue<Vector2> { }
}
