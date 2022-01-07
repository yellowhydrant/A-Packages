using A.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CMI = A.UI.CtxMenuItem<A.Inventory.AInventoryItem>;
using Linq = System.Linq.Enumerable;

namespace A.Inventory.UI
{
    public class AInventoryUI : MonoBehaviour
    {
        public AInventory inventory;

        [SerializeField] AInventorySlotUI slotPrefab;
        [SerializeField] RectTransform slotContainer;

        [SerializeField] AContextualMenuUI contextMenu;

        AInventorySlotUI[] slots;
        RectTransform rect;

        System.Action<int> onClick;
        int selectedSlot = -1;

        private void Awake()
        {
            rect = transform as RectTransform;
            inventory.Init();
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
                slot.gridPosition = inventory.GetPositionFromIndex(i);
                slot.SetItem(inventory.GetItem(i));
                slot.onClick.AddListener(() => onClick.Invoke(i));
                slots[i] = slot;
            }
            onClick = OpenContextMenu;
            inventory.onSlotChanged += (i) => slots[i].SetItem(inventory.GetItem(i));
        }

        //Make it so that depending on capabilities of item in this slot the options get grayed out or show message
        void OpenContextMenu(int index)
        {
            var item = inventory.GetItem(index);
            if (item == null)
                return;
            var defaultCMIs = new CMI[] { new CMI("Examine", null, Examine), new CMI("Move", null, Move), new CMI("Remove", null, Remove), null };
            var slot = slots[index];
            if (item.HasItem)
                contextMenu.SetupButtons(item, Linq.ToArray(Linq.Concat(defaultCMIs, item.Item.actions)));
            //set ctx menu position to slot pos and check for edges
            contextMenu.gameObject.SetActive(true);
        }

        void Examine(AInventoryItem item)
        {
            //do nothing yet
            Debug.Log("This is not implemented");
            CloseContextMenu();
        }

        void Move(AInventoryItem item)
        {
            if (!item.Item.capabilities.HasFlag(AItem.Capabilities.Movable))//
            {
                CloseContextMenu();
                return;
            }
            selectedSlot = inventory.GetIndexFromPosition(item.position);
            slots[selectedSlot].Highlight(true);
            CloseContextMenu();
            onClick = MoveTo;
        }

        void MoveTo(int index)
        {
            if (!inventory.GetItem(index).Item.capabilities.HasFlag(AItem.Capabilities.Movable))//
                return;
            slots[selectedSlot].Highlight(false);
            inventory.SwapItems(index, selectedSlot);
            onClick = OpenContextMenu;
        }

        void Remove(AInventoryItem item)
        {
            if (!item.Item.capabilities.HasFlag(AItem.Capabilities.Removable))//
            {
                CloseContextMenu();
                return;
            }
            inventory.RemoveItem(item.position);
            CloseContextMenu();
        }

        void CloseContextMenu()
        {
            contextMenu.gameObject.SetActive(false);
        }
    }
}
