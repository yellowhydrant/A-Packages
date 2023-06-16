using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/Float/Float List")]
    public class FloatList : ASavableListValue<float> { }
}
