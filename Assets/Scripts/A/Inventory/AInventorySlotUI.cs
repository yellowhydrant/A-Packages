using A.UI;
using TMPro;
using UnityEngine.UI;

namespace A.Inventory.UI
{
    public class AInventorySlotUI : AButton
    {
        public Image itemIcon;
        public Image itemRarity;
        public TMP_Text amountText;

        public AInventoryItem slot;

        public void UpdateVisuals()
        {
            if(itemIcon != null)
                itemIcon.sprite = slot.item.sprite;
            if(itemRarity != null)
                itemRarity.sprite = null;//change later to be actual sprite
            if(amountText != null)
                amountText.text = slot.currentAmount.ToString();
            if (mainText != null)
                mainText.text = slot.item.name;
        }
    }
}

