using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Bool/Bool List")]
    public class BooleanList : ASavableListValue<bool> { }
}
