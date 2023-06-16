using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Bool/Bool Value")]
    public class BooleanValue : ASavableSimpleValue<bool> { }
}
