using A.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace A.Inventory.UI
{
    public class AInventorySlotUI : AButton
    {
        public bool HasItem { get; private set;}

        [SerializeField] Image rarity;
        [SerializeField] Image icon;
        [SerializeField] Image highlight;

        [SerializeField] Sprite defaultIcon;
        [SerializeField] Sprite defaultRarity;
        [SerializeField] string amountFormat = "{0}";

        [SerializeField] AItemRaritySpriteDictionary raritySprites;

        public void SetItem(APositionedItemStack item)
        {
            if (!item.HasItem)
            {
                icon.sprite = defaultIcon;
                rarity.sprite = defaultRarity;
                mainText.text = null;
                interactable = false;
                HasItem = false;
                mainText.text = item.position.ToString();
            }
            else
            {
                icon.sprite = item.stack.item.sprite;
                rarity.sprite = raritySprites.values[item.stack.item.rarity];
                mainText.text = string.Format(amountFormat, item.stack.currentAmount, item.MaxAmount);
                interactable = true;
                HasItem = true;
            }
        }


        public void Highlight(bool state)
        {
            highlight.gameObject.SetActive(state);
        }

        //public void Log(string msg)
        //{
        //    print(msg);
        //}
    }
}
