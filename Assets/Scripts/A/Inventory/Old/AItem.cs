using UnityEngine;

namespace A.Inventory
{
    [CreateAssetMenu(menuName = "A/Inventory/Item")]
    public abstract class AItem : ScriptableObject
    {
        public new string name;
        public string description;

        public string guid = System.Guid.NewGuid().ToString();

        public Sprite sprite;

        public bool showInInventory = true;

        public Rarity rarity;
        public Tags tags;

        public Capabilities capabilities;

        public int maxStoredAmount;

        public int maxUseCount;
        protected int currentUseCount;

        public System.Action<AItem> RemoveItemFromInventory;

        public System.Action onAdd;
        public System.Action onRemove;

        /// <summary>
        /// Item capabilities intended for use in UI.
        /// </summary>
        [System.Serializable, System.Flags]
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
            Movable = 1 << 3,
            /// <summary>
            /// Can this item be seen in the inventory?
            /// </summary>
            Visible = 1 << 4
        }

        [System.Serializable, System.Flags]
        public enum Tags
        {

        }

        // Maybe use the int part of the enum to store the color
        // To do so you can use the first byte for order in enum
        // And the last 3 bytes for an 8bit rgb color

        public enum Rarity
        {
            None,
            Common,
            Uncommon,
            Rare,
            Very_Rare,
            Ultra_Rare,
            Quest,
            Unique
        }

        //public enum Rarity
        //{
        //    None,
        //    Common,
        //    Uncommon,
        //    Rare,
        //    Epic,
        //    Legendary,
        //    Exotic,
        //    Mythic,
        //    Quest,
        //    Unique
        //}

        public void UseItem()
        {
            currentUseCount++;
            OnUse();
            if (currentUseCount == maxUseCount)
                RemoveItemFromInventory(this);
        }

        protected abstract void OnUse();
    }
}
