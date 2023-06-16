using UnityEngine;

namespace A.Saving.Values
{
    [System.Serializable, CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ASavableValueConstants.AssetMenuRoot + "/String/String Array")]
    public class StringArray : ASavableArrayValue<string> { }
}
