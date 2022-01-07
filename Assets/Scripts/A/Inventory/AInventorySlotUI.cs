using A.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace A.Inventory.UI
{
    public class AInventorySlotUI : AButton
    {
        public RectTransform rect;
        public Vector2 gridPosition;

        [SerializeField] Image rarity;
        [SerializeField] Image icon;
        [SerializeField] Image highlight;

        [SerializeField] Sprite defaultIcon;
        [SerializeField] Sprite defaultRarity;
        [SerializeField] string amountFormat = "{0}";

        AInventoryItem item;

        protected override void Awake()
        {
            rect = transform as RectTransform;
        }

        public void SetItem(AInventoryItem item)
        {
            if (!item.HasItem)
            {
                icon.sprite = defaultIcon;
                rarity.sprite = defaultRarity;
                mainText.text = null;
                enabled = false;
            }
            else
            {
                icon.sprite = item.Item.sprite;
                rarity.sprite = null;//
                mainText.text = string.Format(amountFormat, item.currentAmount, item.MaxAmount);
                enabled = true;
            }
        }


        public void Highlight(bool state)
        {
            highlight.gameObject.SetActive(state);
        }

    }
}
