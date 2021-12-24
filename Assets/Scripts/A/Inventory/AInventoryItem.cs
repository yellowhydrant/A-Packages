using UnityEngine;

namespace A.Inventory
{
    [System.Serializable]
    public class AInventoryItem
    {
        public Vector2Int position;

        public AItem item;
        public int currentAmount;
        public int MaxAmount => item.maxStoredAmount;
        public int Capacity => MaxAmount - currentAmount;
        public bool IsFull => currentAmount == MaxAmount;
        public bool IsEmpty => currentAmount == 0;

        public AInventoryItem(AItem itemToStore)
        {
            item = itemToStore;
        }
        public AInventoryItem(AItem itemToStore, int amount)
        {
            item = itemToStore;
            currentAmount = amount;
        }
    }
}

