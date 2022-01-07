using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Bool/Bool Array")]
    public class BooleanArray : ASavableArrayValue<bool> { }
}
