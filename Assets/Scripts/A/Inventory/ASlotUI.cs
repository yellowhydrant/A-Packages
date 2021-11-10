using System.Collections.Generic;
using A.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace A.Inventory.UI
{
    public class ASlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public RectTransform rect => transform as RectTransform;

        public AInventory.ItemSlot storedItemSlot;
        public AInventoryUI inventoryUI;

        public Vector2Int position;

        [SerializeField] RectTransform storedItemCotainer;
        [SerializeField] Image itemRarity;
        [SerializeField] Image itemIcon;
        [SerializeField] TMP_Text itemAmount;
        [SerializeField] Image highlight;

        AInventory inventory;

        public Dictionary<AItem.Rarity, Sprite> raritySprites;

        public void Start()
        {
            inventory = AInventory.instance;
        }

        public void SetStoredItemSlot(AInventory.ItemSlot slot)
        {
            storedItemSlot = slot;
            if (storedItemSlot == null)
            {
                ResetSlot();
            }
            else
            {
                SetupStoredItem();
            }
        }

        public bool StoresItem()
        {
            return storedItemSlot != null;
        }

        private void ResetSlot()
        {
            storedItemCotainer.gameObject.SetActive(false);
        }

        void SetupStoredItem()
        {
            if (storedItemSlot.storedItem.capabilities.HasFlag(AItem.Capabilities.Visible))
            {
                storedItemCotainer.gameObject.SetActive(true);
                itemRarity.sprite = raritySprites[storedItemSlot.storedItem.rarity];
                itemIcon.sprite = storedItemSlot.storedItem.sprite;
                itemAmount.text = storedItemSlot.storedAmount.ToString();
            }
            else
            {
                ResetSlot();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryUI.curSelectedSlot = this;
            highlight.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.prevSelectedSlot = inventoryUI.curSelectedSlot;
            inventoryUI.curSelectedSlot = null;
            highlight.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!inventoryUI.isMergingSlots && !inventoryUI.isMovingSlot)
            {
                // TODO: open menu with selection between use, move, remove and merge also make it possible to invoke these manually for consoles etc....
            }
            else if (inventoryUI.isMovingSlot && !inventoryUI.isMergingSlots)
            {
                inventory.MoveSlot(inventoryUI.prevSelectedSlot.storedItemSlot, inventoryUI.curSelectedSlot.storedItemSlot, inventoryUI.curSelectedSlot.position);
                inventoryUI.isMovingSlot = false;
            }
            else if (inventoryUI.isMergingSlots && !inventoryUI.isMovingSlot)
            {
                if(!inventory.MergeSlots(inventoryUI.prevSelectedSlot.storedItemSlot, inventoryUI.curSelectedSlot.storedItemSlot))
                {
                    DenyAction();
                }
                else
                {
                    inventoryUI.isMergingSlots = false;
                }
            }
        }

        void DenyAction()
        {
            Debug.Log("Error: You can't do that!");
        }

    }
}
