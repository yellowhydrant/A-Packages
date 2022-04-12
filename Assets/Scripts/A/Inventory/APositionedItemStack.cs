using UnityEngine;

namespace A.Inventory
{
    [System.Serializable]
    public struct APositionedItemStack
    {
        public AItemStack stack;
        public Vector2Int position;

        public int MaxAmount => stack.item.maxStoredAmount;
        public int Capacity => stack.MaxAmount - stack.currentAmount;
        public bool IsFull => stack.currentAmount == MaxAmount;
        public bool IsEmpty => stack.currentAmount == 0;
        public bool HasItem => stack.item != null;

        public APositionedItemStack(AItemStack original, Vector2Int pos)
        {
            stack = original;
            position = pos;
        }

        //public void SetItem(AItem item, bool resetValues = true)
        //{
        //    Item = item;
        //    if (item == default(AItem))
        //        itemGuid = null;
        //    else
        //        itemGuid = item.guid;
        //    if (resetValues)
        //    {
        //        currentAmount = 0;
        //        useCount = 0;
        //    }
        //}

        //public void SetItem(AInventoryItem invItem)
        //{
        //    SetItem(invItem.Item);
        //    currentAmount = invItem.currentAmount;
        //    useCount = invItem.useCount;
        //}

        //public void Reset()
        //{
        //    SetItem(default(AItem));
        //    currentAmount = 0;
        //    useCount = 0;
        //}

        //public AInventoryItem Clone()
        //{
        //    var newItem = new AInventoryItem {};
        //    newItem.SetItem(this);
        //    return newItem;
        //}
    }
}

