using A.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CMA = A.UI.CMA<A.Inventory.APositionedItemStack>;
using Linq = System.Linq.Enumerable;

namespace A.Inventory.UI
{
    public class AInventoryUI : MonoBehaviour
    {
        public APositionedInventory inventory;

        [SerializeField] AInventorySlotUI slotPrefab;
        [SerializeField] RectTransform slotContainer;

        [SerializeField] AContextMenuUI contextMenu;

        AInventorySlotUI[] slots;
        RectTransform rect;

        System.Action<int> onClick;
        int selectedSlot = -1;

        private void Awake()
        {
            rect = transform as RectTransform;
        }

        private void Start()
        {
            var slotSize = rect.sizeDelta / inventory.dimensions;
            slots = new AInventorySlotUI[inventory.Size];
            for (int i = 0; i < inventory.Size; i++)
            {
                var slot = Instantiate(slotPrefab, slotContainer);
                slot.rect.sizeDelta = slotSize;
                slot.rect.anchoredPosition = (inventory.GetPositionFromIndex(i) * slotSize) - (rect.sizeDelta / 2 - slotSize / 2);
                //slot.gridPosition = inventory.GetPositionFromIndex(i);
                var pos = inventory.GetPositionFromIndex(i);
                pos.y = (inventory.dimensions.y - 1) - pos.y;
                slot.SetItem(inventory.GetItem(pos));
                var index = inventory.GetIndexFromPosition(pos);
                slot.onClick.AddListener(() => onClick.Invoke(index));
                slots[index] = slot;
            }
            onClick = OpenContextMenu;
            inventory.onSlotChanged += (i) => slots[i].SetItem(inventory.GetItem(i));
            contextMenu.gameObject.SetActive(false);
        }

        //Make it so that depending on capabilities of item in this slot the options get grayed out or show message
        void OpenContextMenu(int index)
        {
            var item = inventory.GetItem(index);
            if (!item.HasItem)
                return;
            var cmas = new List<CMA> { new CMA("Examine", null, Examine, () => true), new CMA("Move", null, Move, () => HasFlag(item, AItem.Capabilities.Movable)), new CMA("Remove", null, Remove, () => HasFlag(item, AItem.Capabilities.Removable)) };
            if(item.stack.item.actions != null)
            {
                cmas.Add(null);
                cmas.AddRange(item.stack.item.actions);
            }
            contextMenu.SetupButtons(item, cmas, CloseContextMenu);
            //TODO: set ctx menu position to slot pos and check for edges
            //if(!inventory.IsVertical && item.position.x == inventory.dimensions.x - 1)
            //    contextMenu.rect.anchoredPosition = slots[inventory.GetIndexFromPosition(new Vector2Int(inventory.dimensions.x, item.position.y))].rect.anchoredPosition;
            //else if(!inventory.IsHorizontal && item.position.y == inventory.dimensions.y - 1)
            //    contextMenu.rect.anchoredPosition = slots[inventory.GetIndexFromPosition(new Vector2Int(item.position.x, inventory.dimensions.y))].rect.anchoredPosition;
            //else
            contextMenu.rect.anchoredPosition = slots[index].rect.anchoredPosition;

            contextMenu.gameObject.SetActive(true);
        }

        void Examine(APositionedItemStack item)
        {
            //do nothing yet
            Debug.Log("This is not implemented");
        }

        void Move(APositionedItemStack item)
        {
            if (!item.stack.item.capabilities.HasFlag(AItem.Capabilities.Movable))//
            {
                CloseContextMenu();
                return;
            }
            selectedSlot = inventory.GetIndexFromPosition(item.position);
            slots[selectedSlot].Highlight(true);
            onClick = MoveTo;
            ActivateEmptySlots(true);
        }

        void MoveTo(int index)
        {
            var item = inventory.GetItem(index);
            if (item.HasItem && !item.stack.item.capabilities.HasFlag(AItem.Capabilities.Movable))
                return;
            slots[selectedSlot].Highlight(false);
            inventory.SwapSlots(index, selectedSlot);
            onClick = OpenContextMenu;
            ActivateEmptySlots(false);
        }

        void Remove(APositionedItemStack item)
        {
            if (!item.stack.item.capabilities.HasFlag(AItem.Capabilities.Removable))//
            {
                CloseContextMenu();
                return;
            }
            inventory.RemoveItem(item.position);
        }

        void CloseContextMenu()
        {
            contextMenu.gameObject.SetActive(false);
        }

        bool HasFlag(APositionedItemStack item, AItem.Capabilities flag)
        {
            return item.stack.item.capabilities.HasFlag(flag);
        }

        void ActivateEmptySlots(bool value)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if(!slots[i].HasItem)
                    slots[i].interactable = value;
            }
        }
    }
}
