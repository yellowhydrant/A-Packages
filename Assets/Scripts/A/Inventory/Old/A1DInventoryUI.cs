//using UnityEngine;

//namespace A.Inventory.UI
//{
//    public class A1DInventoryUI : MonoBehaviour
//    {
//        public AInventory inventory;

//        [SerializeField] AInventorySlotUI slotUI;
//        [SerializeField] float verticalOffset;

//        public void Awake()
//        {
//            //TODO: Add onremove and onadd callbacks
//            //TODO: Add item actions
//            //TODO: Move on to 2D inventgory UI
//            //TODO: Make shop script
//            //TODO: Make gridbased crafting
//            //TODO: Make regular DST crafting
//            //TODO: Add new features to dialogue graph
//            //TODO: Add behaviour tree with soem changes
//            //TODO: Make character model and main vibe without too much plagiarism
//            //STOP WASTING TIME!!!

//            var items = inventory.GetAllItemAmounts();
//            var slotHeight = slotUI.targetGraphic.rectTransform.sizeDelta.y;
//            var curY = -slotHeight / 2;
//            for (int i = 0; i < items.Length; i++)
//            {
//                if (inventory.IsSlotEmpty(i)) continue;
//                var slot = Instantiate(slotUI, transform);
//                slot.targetGraphic.rectTransform.anchoredPosition = new Vector2(0, curY);
//                curY -= slotHeight + verticalOffset;
//                slot.slot = items[i];
//                slot.UpdateVisuals();
//            }
//        }
//    }
//}

