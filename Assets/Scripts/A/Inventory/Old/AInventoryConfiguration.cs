//using System.Collections.Generic;
//using A.Extensions;
//using UnityEngine;

//namespace A.Inventory
//{
//    [CreateAssetMenu(menuName = "A/Inventory/Inventory Configuration")]
//    public class AInventoryConfiguration : AConfiguration
//    {

//        public bool showSlotCapacity = true;
//        public bool highlightOnHover = true;

//        //FIXME: Make this a little less messy ?
//        [SerializeField] SpriteRarity[] spriteRarities;

//        [System.Serializable]
//        public struct SpriteRarity
//        {
//            public AItem.Rarity rarity;
//            public Sprite sprite;
//        }

//        private void Awake()
//        {
//            if (Application.isEditor && !Application.isPlaying)
//            {
//                Debug.Log("Editor Awake!");
//                var maxRarity = AEnumExtensions.GetMaxValue<AItem.Rarity>();
//                spriteRarities = new SpriteRarity[maxRarity];
//                for (int i = 0; i < spriteRarities.Length; i++)
//                {
//                    spriteRarities[i].rarity = (AItem.Rarity)i;
//                }
//            }
//            else if(Application.isPlaying)
//            {
//                Debug.Log("Runtime Awake!");
//            }
//        }
//    }
//}
