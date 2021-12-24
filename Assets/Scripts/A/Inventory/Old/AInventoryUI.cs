using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace A.Inventory.UI
{
    public class AInventoryUI : MonoBehaviour
    {
        public ASlotUI prevSelectedSlot;
        public ASlotUI curSelectedSlot;

        public bool isMovingSlot;
        public bool isMergingSlots;

        [SerializeField] EventSystem eventSystem;

        [SerializeField] Image backgroundImage;
        [SerializeField] Vector2 padding;
        [SerializeField] Vector2 offset;

        [SerializeField] ASlotUI slotUiPrefab;
        [SerializeField] RectTransform slotCotainer;

        ASlotUI[,] slots;

        AInventory inventory;

        private void Awake()
        {
            inventory = AInventory.instance;

            var slotSize = slotUiPrefab.rect.sizeDelta;
            backgroundImage.rectTransform.sizeDelta = (inventory.size * slotSize) + (offset * inventory.size) + padding;

            slots = new ASlotUI[inventory.size.x, inventory.size.y];
            for (int x = 0; x < inventory.size.x; x++)
            {
                for (int y = 0; y < inventory.size.y; y++)
                {
                    var slot = Instantiate(slotUiPrefab, slotCotainer);
                    slot.rect.anchoredPosition = (new Vector2(x + .5f, y + .5f) * slotSize) + (offset * new Vector2(x, y)) - (backgroundImage.rectTransform.sizeDelta / 2) + padding / 2 + (offset / 2);
                    slot.position = new Vector2Int(x, y);
                    slot.inventoryUI = this;

                    slots[x, y] = slot;
                }
            }

            inventory.RegisterCallback(UpdateStoredItems);
            inventory.onItemsMoved += OnItemsMoved;
            inventory.onItemChanged += OnItemChanged;
        }

        void OnItemsMoved(Vector2Int from, Vector2Int to)
        {
            slots[to.x, to.y].SetStoredItemSlot(inventory.GetSlot(from));
            slots[from.x, from.y].SetStoredItemSlot(null);
        }

        void OnItemChanged(AInventory.ItemSlot slot)
        {
            slots[slot.position.x, slot.position.y].SetStoredItemSlot(slot);
        }

        void UpdateStoredItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (AInventory.ItemSlot item in e.NewItems)
            {
                for (int x = 0; x < slots.GetLength(0); x++)
                {
                    for (int y = 0; y < slots.GetLength(1); y++)
                    {
                        if (!slots[x, y].StoresItem())
                        {
                            item.position = new Vector2Int(x, y);
                            goto end;
                        }
                    }
                }
                end:
                    slots[item.position.x, item.position.y].SetStoredItemSlot(item);
            }

            foreach (AInventory.ItemSlot item in e.OldItems)
            {
                slots[item.position.x, item.position.y].SetStoredItemSlot(item);
            }
        }
    }
}
