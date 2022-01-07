using UnityEngine;

namespace A.Inventory
{
    [System.Serializable]
    public class AInventoryItem
    {
#if UNITY_EDITOR
        [field: SerializeField]
#endif
        public AItem Item { get; private set; }
        [System.NonSerialized]
        public Vector2Int position;
        [field: SerializeField, HideInInspector]
        public string itemGuid { get; private set; }
        public int currentAmount;
        public int useCount;
        public int MaxAmount => Item.maxStoredAmount;
        public int Capacity => MaxAmount - currentAmount;
        public bool IsFull => currentAmount == MaxAmount;
        public bool IsEmpty => currentAmount == 0;
        public bool HasItem => Item != null;

        public AInventoryItem(Vector2Int pos)
        {
            position = pos;
        }

        //public AInventoryItem(AItem itemToStore)
        //{
        //    Item = itemToStore;
        //}
        public AInventoryItem(AItem itemToStore, int amount)
        {
            Item = itemToStore;
            currentAmount = amount;
        }
        public void SetItem(AItem item)
        {
            Item = item;
            if (item == default(AItem))
                itemGuid = null;
            else
                itemGuid = item.guid;
            currentAmount = 0;
            useCount = 0;
        }

        public void SetItem(AInventoryItem invItem)
        {
            SetItem(invItem.Item);
            currentAmount = invItem.currentAmount;
            useCount = invItem.useCount;
        }

        public void Reset()
        {
            SetItem(default(AItem));
            currentAmount = 0;
            useCount = 0;
        }
    }
}

