using A.Saving;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace A.Inventory
{
    //TODO: Add custom sorting
    [System.Serializable]
    public class AInventoryBase : ASavableObject
    {
        public AItemStack[] itemsToAddOnInit;

        public ReadOnlyCollection<AItemStack> Slots => new ReadOnlyCollection<AItemStack>(slots);
        public IReadOnlyCollection<AItemStack> ReservedItems => reservedItems;
        [field: SerializeField]
        public int capacity { get; protected set; }

        public bool HasItemsInQueue => reservedItems.Count > 0;
        public override string DataSlotSubDirectory => "Inventories";
        public System.Action<int> onSlotChanged;

        public const int UnlimitedCapacity = -1;

        [field: SerializeField, MyBox.ReadOnly]
        protected List<AItemStack> slots = new List<AItemStack>();
        protected Queue<AItemStack> reservedItems = new Queue<AItemStack>();

        public enum SortingCriteria
        {
            None
        }

        virtual protected void OnEnable()
        {
            Init();
        }

        virtual public void Init(int capacity = -1)
        {
            //Guard clause
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
#endif
            if (this.capacity == 0)
                this.capacity = capacity;

            //Initialize all slots with empty AInventoryItem's
            if (this.capacity != UnlimitedCapacity)
                slots = new List<AItemStack>(new AItemStack[this.capacity]);

            //Enqueue all of the editor-assigned items
            for (int i = 0; i < itemsToAddOnInit.Length; i++)
            {
                itemsToAddOnInit[i].currentAmount = Mathf.Min(itemsToAddOnInit[i].currentAmount, itemsToAddOnInit[i].item.maxStoredAmount);
                if (itemsToAddOnInit[i].HasItem)
                    reservedItems.Enqueue(itemsToAddOnInit[i]);
            }
            //Add the items that fit
            TryDequeueReservedItems();
        }

        #region Adding, Removing and Swapping
        //Returns true if the added amount fit in the inventory
        public bool AddItem(AItem item, int amount = 1)
        {
            //Fill the slots with items
            var leftoverAmount = ChangeItemAmount(item, amount, true);
            //If any items remain divide them into slots and add them to a queue
            if (leftoverAmount > 0)
            {
                while(leftoverAmount != 0)
                {
                    var amountToReserve = Mathf.Min(leftoverAmount, item.maxStoredAmount);
                    leftoverAmount -= amountToReserve;
                    reservedItems.Enqueue(new AItemStack(item, amountToReserve));
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SwapSlots(int a, int b)
        {
            var itemA = slots[a];
            slots[a] = slots[b];
            slots[b] = itemA;
            onSlotChanged?.Invoke(a);
            onSlotChanged?.Invoke(b);
        }

        public void RemoveSlot(int index)
        {
            slots[index] = default;
            onSlotChanged?.Invoke(index);
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
            if (GetTotalAmount(item) < amount)
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
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item == reservedItems.Peek().item)
                {
                    Debug.Log(slots[i].Capacity);
                    if (slots[i].Capacity >= reservedItems.Peek().currentAmount)
                        slots[i] += reservedItems.Dequeue().currentAmount;
                }
                else
                {
                    slots[i] = reservedItems.Dequeue();
                    onSlotChanged?.Invoke(i);
                }
                if (reservedItems.Count == 0)
                    return;
            }

            if(capacity == UnlimitedCapacity)
            {
                for (int i = 0; i < reservedItems.Count; i++)
                {
                    slots.Add(reservedItems.Dequeue());
                }
            }
        }

        int ChangeItemAmount(AItem item, int amount, bool add)
        {
            //Reverse loop trough items
            //Remove any items in the slot
            //or
            //Fill the slots that aren't full yet
            for (int i = slots.Count - 1; i >= 0; i--)
            {
                //If the slot has an item and you're removing or you're adding and it's not full yet proceed
                if (slots[i].HasItem && ((add && !slots[i].IsFull) || (!add)))
                {
                    if (ChangeAmount(i))
                        return 0;
                }
            }
            //If you're not adding return the remainder
            if (!add) return amount;

            //Loop trough items again top to bottom
            for (int i = 0; i < slots.Count; i++)
            {
                //Fill any empty slots with the remaining items
                if (!slots[i].HasItem)
                {
                    slots[i] = new AItemStack(item, 0);
                    if (ChangeAmount(i))
                        return 0;
                }
            }

            //If inventory capacity is equal to infinity add new slots with the remainder
            if(capacity == UnlimitedCapacity)
            {
                while(amount != 0)
                {
                    var fillAmount = Mathf.Min(amount, item.maxStoredAmount);
                    amount -= fillAmount;
                    slots.Add(new AItemStack(item, fillAmount));
                }
            }


            bool ChangeAmount(int index)
            {
                //Pick the smallest number out of the amount left and the capacity or currentamount depending on operation
                var amountToChange = Mathf.Min(amount, add ? slots[index].Capacity : slots[index].currentAmount);
                //Change the currentAmount and amount
                slots[index] += amountToChange * (add ? 1 : -1);
                amount -= amountToChange;
                //If the slot is completely empty reset the slot
                if (slots[index].IsEmpty)
                    slots[index] = default;
                onSlotChanged?.Invoke(index);
                return amount == 0;
            }

            //If all slots are occupied return the remainder
            return amount;
        }
        #endregion

        #region Utility Methods
        public int GetTotalAmount(AItem item)
        {
            //Loop through all slots with this item and add the currentAmount to the total
            var amount = 0;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].item == item)
                    amount += slots[i].currentAmount;
            }
            return amount;
        }

        public HashSet<AItem> GetUniqueItems()
        {
            var hashtable = new HashSet<AItem>();
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots != null)
                    hashtable.Add(slots[i].item);
            }
            return hashtable;
        }

        public AItemStack[] GetAllItemAmounts()
        {
            var hash = System.Linq.Enumerable.ToArray(GetUniqueItems());
            var arr = new AItemStack[hash.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = new AItemStack(hash[i], GetTotalAmount(hash[i]));
            }
            return arr;
        }

        public bool HasItem(AItem item) => GetTotalAmount(item) > 0;

        public bool HasItem(AItem item, out int amount)
        {
            amount = GetTotalAmount(item);
            return amount > 0;
        }
        #endregion

        #region Saving & Loading
        public override void ResetValue()
        {
            Init();
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(new Wrapper { Items = slots.ToArray(), ReservedItems = reservedItems.ToArray() });
        }

        public override void FromJson(string json)
        {
            var wrapper = JsonUtility.FromJson<Wrapper>(json);
            slots = new List<AItemStack>(wrapper.Items);
            reservedItems = new Queue<AItemStack>(wrapper.ReservedItems);
        }

        class Wrapper
        {
            public AItemStack[] Items;
            public AItemStack[] ReservedItems;
        }
        #endregion
    }
}
