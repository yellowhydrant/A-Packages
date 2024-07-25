using UnityEngine;

namespace A.Inventory
{
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + AInventoryConstants.AssetMenuRoot + "/Item Rarity Sprite Dictionary")]
    public class AItemRaritySpriteDictionary : AEnumObjectDictionary<AItem.Rarity, Sprite>
    {

    }
}

#if UNITY_EDITOR
namespace A.Editor
{
    [UnityEditor.CustomEditor(typeof(Inventory.AItemRaritySpriteDictionary))]
    public class AItemRaritySpriteDictionaryEditor : AEnumObjectDictionaryEditor<Inventory.AItem.Rarity, Sprite>
    {

    }
}
#endif