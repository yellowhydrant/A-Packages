using A.Saving;
using A.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Inventory
{
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + AInventoryConstants.AssetMenuRoot +  "/Inventory")]
    public class AInventory : ASavableObject
    {
        public Vector2Int dimensions = Vector2Int.one;
        public int Size => dimensions.x * dimensions.y;
        public bool IsSingleSlot => dimensions.x == 1 && dimensions.y == 1;
        public bool IsVertical => dimensions.x == 1;
        public bool IsHorizontal => dimensions.y == 1;
        public bool HasItemsInQueue => reservedItems.Count > 0;

        public AInventoryItem[] itemsToAddOnInit;
        //bool hasInitialized;

        AInventoryItem[] items;
        Queue<AInventoryItem> reservedItems = new Queue<AInventoryItem>();

        public override string SaveSlotSubDirectory => "";
        public System.Action<int> onSlotChanged;

        //TODO: Add custom sorting
        public enum SortingCriteria
        {
            None
        }

        private void OnEnable()
        {
            Init();
        }

        public void Init()
        {
            //Guard clause
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            //Initialize all slots with empty AInventoryItem's
            items = new AInventoryItem[Size];
            for (int i = 0; i < items.Length; i++)
                items[i] = new AInventoryItem(GetPositionFromIndex(i));
            //Enqueu all of the editor-assigned items
            for (int i = 0; i < itemsToAddOnInit.Length; i++)
            {
                if(itemsToAddOnInit != null)
                    reservedItems.Enqueue(itemsToAddOnInit[i]);
            }
            //Add the items that fit
            TryDequeueReservedItems();
        }

        public bool AddItem(AItem item, int amount = 1)
        {
            //Fill the slots with items
            var leftoverAmount = ChangeItemAmount(item, amount, true);
            //If any items remain divide them into slots and add them to a queue
            if(leftoverAmount > 0)
            {
                for (int i = 0; i < leftoverAmount / item.maxStoredAmount; i++)
                {
                    var amountToReserve = Mathf.Min(item.maxStoredAmount);
                    reservedItems.Enqueue(new AInventoryItem(item, amountToReserve));
                }
                if (leftoverAmount % item.maxStoredAmount != 0)
                    reservedItems.Enqueue(new AInventoryItem(item, leftoverAmount % item.maxStoredAmount));

                return false;
            }
            else
            {
                return true;
            }
        }

        public void SwapItems(int a, int b)
        {
            var itemA = items[a].Clone();
            items[a].SetItem(items[b]);
            items[b].SetItem(itemA);
            onSlotChanged?.Invoke(a);
            onSlotChanged?.Invoke(b);
        }

        public void RemoveItem(int index)
        {
            items[index].Reset();
            onSlotChanged?.Invoke(index);
        }

        public void RemoveItem(Vector2Int position)
        {
            RemoveItem(GetIndexFromPosition(position));
        }

        public void RemoveItem(AItem item, int amount = 1)
        {
            //Remove items from the slots bottom to top
            ChangeItemAmount(item, amount, false);
            //Dequeue items to fill the empty space
            TryDequeueReservedItems();
        }

        public bool TryRemoveItem(AItem item, int amount)
        {
            return TryRemoveItem(item, amount, out int r);
        }

        public bool TryRemoveItem(AItem item, int amount, out int remainder)
        {
            if(GetTotalAmount(item) < amount)
            {
                remainder = amount;
                return false;
            }
            else
            {
                remainder = 0;
                RemoveItem(item);
                return true;
            }
        }

        //Change this so it uses the add method and doesn't just fill compeltely empty slots
        void TryDequeueReservedItems()
        {
            if (reservedItems.Count == 0) 
                return;

            //Loop trough items and fill any open slots with enqeued items
            for (int i = 0; i < items.Length; i++)
            {
                if (!items[i].HasItem)
                {
                    if (reservedItems.Count > 0)
                    {
                        items[i].SetItem(reservedItems.Dequeue());
                        onSlotChanged?.Invoke(i);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        int ChangeItemAmount(AItem item, int amount, bool add)
        {
            //Reverse loop trough items
            //Remove any items in the slot
            //or
            //Fill the slots that aren't full yet
            for (int i = items.Length - 1; i >= 0; i--)
            {
                //If the slot has an item and you're removing or you're adding and it's not full yet proceed
                if (items[i].HasItem && ((add && !items[i].IsFull) || (!add)))
                {
                    if (ChangeAmount(i))
                        return 0;
                }
            }
            //If you're not adding return the remainder
            if (!add) return amount;

            //Loop trough items again top to bottom
            for (int i = 0; i < items.Length; i++)
            {
                //Fill any empty slots with the remaining items
                if (!items[i].HasItem)
                {
                    items[i].SetItem(item);
                    if (ChangeAmount(i))
                        return 0;
                }
            }

            bool ChangeAmount(int index)
            {
                //Pick the smallest number out of the amount left and the capacity or currentamount depending on operation
                var amountToChange = Mathf.Min(amount, add ? items[index].Capacity : items[index].currentAmount);
                //Change the currentAmount and amount
                items[index].currentAmount += amountToChange * (add ? 1 : -1);
                amount -= amountToChange;
                //If the slot is completely empty reset the slot
                if (items[index].IsEmpty)
                    items[index].Reset();
                onSlotChanged?.Invoke(index);
                return amount == 0;
            }

            //If all slots are occupied return the remainder
            return amount;
        }

        public int GetTotalAmount(AItem item)
        {
            //Loop trough all slots with this item and add the currentAmount to the total
            var amount = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Item == item)
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

        public bool IsSlotEmpty(int index) => !items[index].HasItem;

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
                    hashtable.Add(items[i].Item);
            }
            return hashtable;
        }

        public AInventoryItem[] GetAllItemAmounts()
        {
            var hash = System.Linq.Enumerable.ToArray(GetUniqueItems());
            var arr = new AInventoryItem[hash.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new AInventoryItem(hash[i], GetTotalAmount(hash[i]));
            }
            return arr;
        }

        public int GetIndexFromPosition(Vector2Int pos) => pos.x + dimensions.x * pos.y;
        public Vector2Int GetPositionFromIndex(int index) => new Vector2Int(index % dimensions.x, index / dimensions.x);

        public override void ResetValue()
        {
            Init();
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(new Wrapper { Items = items, ReservedItems = reservedItems.ToArray()});
        }

        public override void FromJson(string json)
        {
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            for (int i = 0; i < wrapper.Items.Length; i++)
                wrapper.Items[i].SetItem(!wrapper.Items[i].itemGuid.IsValidGuid() ? null : AItemCache.Items[wrapper.Items[i].itemGuid], false);
            items = wrapper.Items;
            for (int i = 0; i < wrapper.ReservedItems.Length; i++)
                wrapper.ReservedItems[i].SetItem(!wrapper.ReservedItems[i].itemGuid.IsValidGuid() ? null : AItemCache.Items[wrapper.Items[i].itemGuid], false);
            reservedItems = new Queue<AInventoryItem>(wrapper.ReservedItems);
        }

        class Wrapper
        {
            public AInventoryItem[] Items;
            public AInventoryItem[] ReservedItems;
        }
    }
}

