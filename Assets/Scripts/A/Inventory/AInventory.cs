using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace A.Inventory
{
    [CreateAssetMenu(menuName = "A/Inventory/Inventory")]
    public class AInventory : ScriptableObject
    {
        public Vector2Int dimensions = Vector2Int.one;
        public int Size => dimensions.x * dimensions.y;
        public bool IsMultiDimensional => dimensions.y > 1;
        public bool HasItemsInQueue => reservedItems.Count > 0;

        public AInventoryItem[] itemsToAddOnInit;
        bool hasInitialized;

        AInventoryItem[] items;
        Queue<AInventoryItem> reservedItems = new Queue<AInventoryItem>();

        bool isInitialized;

        public enum SortingCriteria
        {
            None
        }

        void Awake()
        {
            Init();
        }

        public void Init()
        {
            //Guard clause
            if (hasInitialized) return;
            hasInitialized = true;

            items = new AInventoryItem[Size];
            for (int i = 0; i < itemsToAddOnInit.Length; i++)
            {
                if(itemsToAddOnInit != null)
                    reservedItems.Enqueue(itemsToAddOnInit[i]);
            }
            TryDequeueReservedItems();
        }

        public bool AddItem(AItem item, int amount = 1)
        {
            var leftoverAmount = ChangeItemAmount(item, amount, true);
            if(leftoverAmount > 0)
            {
                for (int i = 0; i < leftoverAmount / item.maxStoredAmount; i++)
                {
                    var amountToReserve = Mathf.Min(item.maxStoredAmount);
                    reservedItems.Enqueue(new AInventoryItem(item) { currentAmount = amountToReserve });
                }
                if (leftoverAmount % item.maxStoredAmount != 0)
                    reservedItems.Enqueue(new AInventoryItem(item) { currentAmount = leftoverAmount % item.maxStoredAmount });

                return false;
            }
            else
            {
                return true;
            }
        }

        public bool RemoveItem(AItem item, int amount = 1)
        {
            var result = ChangeItemAmount(item, amount, false) == 0;
            TryDequeueReservedItems();
            return result;
        }

        void TryDequeueReservedItems()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].IsEmpty)
                {
                    if (reservedItems.Count > 0)
                        items[i] = reservedItems.Dequeue();
                    else
                        items[i] = null;
                }
            }
        }

        int ChangeItemAmount(AItem item, int amount, bool polarity)
        {
            for (int i = items.Length - 1; i >= 0; i--)
            {
                if (items[i] != null && ((polarity && !items[i].IsFull) || (!polarity)))
                {
                    if (ChangeAmount(i))
                        return 0;
                }
            }

            if (!polarity) return amount;

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    items[i] = new AInventoryItem(item);
                    items[i].position = GetPositionFromIndex(i);
                    if (ChangeAmount(i))
                        return 0;
                }
            }

            bool ChangeAmount(int index)
            {
                var amountToChange = Mathf.Min(amount, polarity ? items[index].Capacity : items[index].currentAmount);
                items[index].currentAmount += amountToChange * (polarity ? 1 : -1);
                amount -= amountToChange;
                return amount == 0;
            }

            return amount;
        }

        public int GetTotalAmount(AItem item)
        {
            var amount = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] != null && items[i].item == item)
                    amount += items[i].currentAmount;
            }
            return amount;
        }

        public bool HasItem(AItem item) => GetTotalAmount(item) > 0;

        public bool HasItem(AItem item, out int amount)
        {
            amount = GetTotalAmount(item);
            return amount > 0;
        }

        public bool IsSlotEmpty(int index)
        {
            var slot = items[index];
            if(slot == null || slot.item == null || slot.IsEmpty)
            {
                items[index] = null;
                return true;
            }
            return false;
        }

        public void SortInventory(SortingCriteria type)
        {
            switch (type)
            {
                case SortingCriteria.None:
                    System.Array.Sort(items);
                    break;
                default:
                    break;
            }
        }

        public AInventoryItem GetItem(Vector2Int position) => items[GetIndexFromPosition(position)];

        public AInventoryItem GetItem(int index) => items[index];

        public HashSet<AItem> GetUniqueItems()
        {
            var hashtable = new HashSet<AItem>();
            for (int i = 0; i < items.Length; i++)
            {
                if(items != null)
                    hashtable.Add(items[i].item);
            }
            return hashtable;
        }

        public AInventoryItem[] GetAllItemAmounts()
        {
            var hash = GetUniqueItems().ToArray();
            var arr = new AInventoryItem[hash.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new AInventoryItem(hash[i], GetTotalAmount(hash[i]));
            }
            return arr;
        }

        public int GetIndexFromPosition(Vector2Int pos) => pos.x + dimensions.x * pos.y;
        public Vector2Int GetPositionFromIndex(int index) => new Vector2Int(index % dimensions.x, index / dimensions.x);
    }
}

