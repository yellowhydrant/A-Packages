using System;
using System.Collections.Generic;
using A.UI;
using UnityEngine;
using UnityEngine.UI;

namespace A.UI
{
    public class AContextMenuUI : MonoBehaviour
    {
        public RectTransform rect;

        [SerializeField] Vector2Int direction;
        [SerializeField] AButton buttonPrefab;
        [SerializeField] RectTransform separatorPrefab;
        [SerializeField] RectTransform container;

        private void Awake()
        {
            rect = transform as RectTransform;
        }

        public void SetupButtons<T>(T obj, IEnumerable<CMI<T>> items, Action endAct)
        {
            //Clean up previous actions
            foreach (Transform child in container)
                Destroy(child.gameObject);

            //Store position to align items in direction
            var deltaPos = Vector2.zero;
            foreach(var item in items)
            {
                RectTransform rect;
                if (item != null)
                {
                    var button = Instantiate(buttonPrefab, container);
                    rect = button.rect;
                    if(button.mainText != null)
                        button.mainText.text = item.label;
                    if((button.targetGraphic as Image) != null)
                        (button.targetGraphic as Image).sprite = item.sprite;
                    button.onClick.AddListener(() => { item.action.Invoke(obj); endAct.Invoke(); });
                    button.interactable = item.validate.Invoke();
                }
                else
                {
                    rect = Instantiate(separatorPrefab, container);
                }
                deltaPos += rect.sizeDelta * direction;
                rect.anchoredPosition = deltaPos + (rect.sizeDelta * direction) / -2f;
            }
        }
    }

    [Serializable]
    public class CMI<T>
    {
        public string label;
        public Sprite sprite;
        public Action<T> action;
        public Func<bool> validate;

        public CMI(string label, Sprite sprite, Action<T> action, Func<bool> validate)
        {
            this.label = label;
            this.sprite = sprite;
            this.action = action;
            this.validate = validate;
        }
    }
}

