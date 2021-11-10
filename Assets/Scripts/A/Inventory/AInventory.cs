using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using A;
using UnityEngine;

namespace A.Inventory
{
    //TODO: Make UI to represent the inventory visually and add manual control
    [AddComponentMenu("A/Singletons/Inventory")]
    public class AInventory : ASingleton<AInventory>
    {
        public Vector2Int size;
        public int maxSlotAmount => size.x * size.y;

        public System.Action<Vector2Int, Vector2Int> onItemsMoved;
        public System.Action<ItemSlot> onItemChanged;

        ObservableCollection<ItemSlot> itemSlots = new ObservableCollection<ItemSlot>();

        public class ItemSlot
        {
            public Vector2Int position;
            public AItem storedItem;
            public int storedAmount;

            public bool isFull => storedAmount == storedItem.maxStoredAmount;
            public int capacity => storedItem.maxStoredAmount - storedAmount;
            public int maxCapacity => storedItem.maxStoredAmount;

            public ItemSlot(AItem item, int amount = 1)
            {
                storedItem = item;
                storedAmount = amount;
            }
        }

        //Implement hidden item slots

        public bool AddItem(AItem item, int amount = 1)
        {
            var availableSlots = itemSlots.Where((slot) => slot.storedItem == item && !slot.isFull).ToList();
            var neededEmptySlots = Mathf.CeilToInt(amount / (float)item.maxStoredAmount);

            if (itemSlots.Count == maxSlotAmount
                && !availableSlots.Any()
                && maxSlotAmount - itemSlots.Count >= neededEmptySlots)
                return false;

            if (availableSlots != null)
            {
                availableSlots.OrderByDescending((slot) => slot.storedAmount);
                foreach (var slot in availableSlots)
                {
                    var amountToAdd = slot.capacity > amount ? amount : slot.capacity;
                    amount -= amountToAdd;
                    slot.storedAmount += amountToAdd;
                    if (amount == 0)
                        break;
                }
            }
            else
            {
                for (int i = 1; i <= neededEmptySlots; i++)
                {
                    if (i == neededEmptySlots)
                        itemSlots.Add(new ItemSlot(item, amount % item.maxStoredAmount));
                    else
                        itemSlots.Add(new ItemSlot(item, item.maxStoredAmount));
                }
            }

            item.onAdd?.Invoke(); // Change later maybe

            item.RemoveItemFromInventory = (itemToRemove) => RemoveItem(itemToRemove);

            return true;
        }

        public bool AddItemByGuid(string guid, int amount = 1)
        {
            return AddItem(GetItemByGuid(guid), amount);
        }

        public bool RemoveItem(AItem item, int amount = 1, bool clampAmount = true)
        {
            var slotsWithItem = itemSlots.Where((slot) => slot.storedItem == item);
            var totalStoredAmount = slotsWithItem.Sum((slot) => slot.storedAmount);

            if(clampAmount)
                amount = Mathf.Min(totalStoredAmount, amount);

            if (!slotsWithItem.Any() && totalStoredAmount < amount)
                return false;

            slotsWithItem.OrderBy((slot) => slot.storedAmount);
            foreach (var slot in slotsWithItem)
            {
                var amountToRemove = slot.storedAmount > amount ? amount : slot.storedAmount;
                amount -= amountToRemove;
                slot.storedAmount -= amountToRemove;
                if (amount == 0)
                    break;
            }

            item.onRemove?.Invoke(); //Change later maybe

            itemSlots = itemSlots.Where((slot) => slot.storedAmount > 0) as ObservableCollection<ItemSlot>;

            return true;
        }

        public bool RemoveItemByGuid(string guid, int amount = 1, bool clampAmount = true)
        {
            return RemoveItem(GetItemByGuid(guid), amount, clampAmount);
        }

        public AItem GetItemByGuid(string guid)
        {
            return itemSlots.FirstOrDefault((slot) => slot.storedItem.guid == guid)?.storedItem;
        }

        public bool HasItem(AItem item)
        {
            return itemSlots.Any((slot) => slot.storedItem == item);
        }

        public bool HasItemByGuid(string guid)
        {
            return HasItem(GetItemByGuid(guid));
        }

        public void SortInventory()
        {

        }

        public ItemSlot GetSlot(Vector2Int position)
        {
            return itemSlots.FirstOrDefault((slot) => slot.position == position);
        }

        public ItemSlot GetSlot(int index)
        {
            return index > 0 && index < itemSlots.Count ? itemSlots[index] : null;
        }

        public void MoveSlot(ItemSlot from, ItemSlot to, Vector2Int toPos)
        {
            if (from == to)
            {
                return;
            }
            else if (to == null)
            {
                onItemsMoved?.Invoke(from.position, toPos);
                from.position = toPos;
            }
            else
            {
                onItemsMoved?.Invoke(from.position, to.position);
                onItemsMoved?.Invoke(to.position, from.position);

                to.position = from.position;
                from.position = toPos;
            }
        }

        public bool MergeSlots(ItemSlot from, ItemSlot to)
        {
            if (to.isFull || from == to || from.storedItem != to.storedItem) return false;
            {
                var movedAmount = Mathf.Min(to.capacity, from.storedAmount);
                to.storedAmount += movedAmount;
                from.storedAmount -= movedAmount;
                if (from.storedAmount == 0)
                    itemSlots.Remove(from);
                else
                    onItemChanged?.Invoke(from);
                onItemChanged?.Invoke(to);
            }
            return true;
        }

        public void RegisterCallback(System.Collections.Specialized.NotifyCollectionChangedEventHandler del)
        {
            itemSlots.CollectionChanged += del;
        }
    }
}
