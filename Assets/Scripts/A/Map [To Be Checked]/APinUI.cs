//using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace A.Map
{
    public class APinUI : MonoBehaviour
    {
        public Image icon;
        public Image arrow;
        public RectTransform rect;
        public Vector2 position => rect.anchoredPosition + (rect.parent as RectTransform).anchoredPosition;
        public float angleOffset;

        private void Awake()
        {
            rect = transform as RectTransform;
        }
    }
}
