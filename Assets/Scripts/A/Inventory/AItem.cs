using System;
using System.Collections.Generic;
using A.UI;
using UnityEngine;

namespace A.Inventory
{
    //[CreateAssetMenu(menuName = "A/Inventory/Items/ ")]
    public abstract class AItem : ScriptableObject
    {   
        public new string name;
        public string description;

        public Sprite sprite;

        public Rarity rarity;
        public Tags tags;

        public Capabilities capabilities;

        public int maxStoredAmount;
        public int maxUseCount;

        public string guid { get; private set; }

        public virtual CMA<APositionedItemStack>[] actions => null;
        public Action<AItem, int> RemoveItemFromInventory;

        public enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            Very_Rare,
            Ultra_Rare,
            Quest,
            Unique
        }

        [Serializable, Flags]
        public enum Tags
        {

        }

        /// <summary>
        /// Item capabilities intended for use in UI.
        /// </summary>
        [Serializable, Flags]
        public enum Capabilities
        {
            None = 0 << 0,
            /// <summary>
            /// Can this item be used?
            /// </summary>
            Usable = 1 << 0,
            /// <summary>
            /// Can this item be removed from the inventory?
            /// </summary>
            Removable = 1 << 1,
            /// <summary>
            /// Can this item be stacked with another item in the inventory?
            /// </summary>
            Stackable = 1 << 2,
            /// <summary>
            /// Can this item be moved to a different slot in the inventory?
            /// </summary>
            Movable = 1 << 3
        }

        protected virtual void Awake()
        {
            guid = System.Guid.NewGuid().ToString();
        }

        public override bool Equals(object other)
        {
            if (other is AItem item)
                return item.guid == guid;
            else
                return base.Equals(other);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public static bool operator ==(AItem lhs, AItem rhs)
        {
            if ((object)lhs == null)
            {
                if ((object)rhs == null)
                    return true;
                else
                    return false;
            }
            else
            {
                return lhs.Equals(rhs);
            }
        }

        public static bool operator !=(AItem lhs, AItem rhs)
        {
            return !(lhs == rhs);
        }
    }
}

