using UnityEngine;

namespace A.Inventory
{

    //Valid

    [System.Serializable]
    public struct AItemStack
    {
        //make it so that it auto fetches if itemguid is valid and item_ is null
        public AItem item 
        { 
            get { return FetchItem(); } 
            set { itemGuid = item != null ? item.guid : null; _item = value; } //set guide variable and assign item to internal _item
        }
#if UNITY_EDITOR
        [field: SerializeField]
#endif
        AItem _item;
        [field: SerializeField, HideInInspector]
        public string itemGuid { get; private set; }
        public int currentAmount; //add set check if below 0 throw error
        public int useCount;

        //Utility values
        public int MaxAmount => item.maxStoredAmount;
        public int Capacity => MaxAmount - currentAmount;
        public bool IsFull => currentAmount == MaxAmount;
        public bool IsEmpty => currentAmount == 0;
        public bool HasItem => item != null;


        public AItemStack(AItem item, int amount)
        {
            itemGuid = item.guid;
            currentAmount = amount;
            useCount = 0;
            _item = null;
            this.item = item;
        }

        public AItemStack(AItemStack original, bool copyUseCount) 
            : this(original.item, original.currentAmount)
        {
            if (copyUseCount)
            {
                useCount = original.useCount;
            }
        }

        //public AItemStack(AItemStack original, int amountChange, bool copyUseCount)
        //    : this(original.item, original.currentAmount + amountChange)
        //{
        //    if (copyUseCount)
        //    {
        //        useCount = original.useCount;
        //    }
        //}

        public AItem FetchItem()
        {
            if (_item != null)
                return _item;
            else
                return AItemCache.GetItem(itemGuid);
        }

        public static AItemStack operator +(AItemStack lhs, int rhs)
        {
            lhs.currentAmount += rhs;
            return lhs;
        }
    }
}
