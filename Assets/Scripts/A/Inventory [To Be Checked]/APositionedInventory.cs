using A.Saving;
using A.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Inventory
{
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + AInventoryConstants.AssetMenuRoot +  "/Inventory")]
    [System.Serializable]
    public class APositionedInventory : AInventoryBase
    {
        public Vector2Int dimensions = Vector2Int.one;
        public int Size => dimensions.x * dimensions.y;
        public bool IsSingleSlot => dimensions.x == 1 && dimensions.y == 1;
        public bool IsVertical => dimensions.x == 1;
        public bool IsHorizontal => dimensions.y == 1;

        public override string DataSlotSubDirectory => "";

        override public void Init(int capacity)
        {
            base.Init(Size);
        }

        public void RemoveItem(Vector2Int position) => RemoveSlot(GetIndexFromPosition(position));
        public bool IsSlotEmpty(int index) => !slots[index].HasItem;
        public APositionedItemStack GetItem(Vector2Int position) => new APositionedItemStack(slots[GetIndexFromPosition(position)], position);
        public APositionedItemStack GetItem(int index) => new APositionedItemStack(slots[index], GetPositionFromIndex(index));
        public int GetIndexFromPosition(Vector2Int pos) => pos.x + dimensions.x * pos.y;
        public Vector2Int GetPositionFromIndex(int index) => new Vector2Int(index % dimensions.x, index / dimensions.x);
    }
}

