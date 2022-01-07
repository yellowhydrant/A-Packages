using A.UI;
using UnityEngine;
using UnityEngine.UI;

namespace A.UI
{
    public class AContextualMenuUI : MonoBehaviour
    {
        [SerializeField] Vector2 direction;
        [SerializeField] AButton buttonPrefab;
        [SerializeField] RectTransform separatorPrefab;
        [SerializeField] RectTransform container;

        public void SetupButtons<T>(T obj, params System.Tuple<string, Sprite, System.Action<T>>[] actions)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);
            var step = buttonPrefab.targetGraphic.rectTransform.sizeDelta * direction;
            var deltaPos = step / 2;
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] != null)
                {
                    var button = Instantiate(buttonPrefab, container);
                    button.targetGraphic.rectTransform.anchoredPosition = deltaPos;
                    if(button.mainText != null)
                        button.mainText.text = actions[i].Item1;
                    (button.targetGraphic as Image).sprite = actions[i].Item2;
                    if (actions[i].Item3 != null)
                        button.onClick.AddListener(() => actions[i].Item3?.Invoke(obj));
                    else
                        button.enabled = false;
                }
                else
                {
                    var sep = Instantiate(separatorPrefab, container);//add support for any separator size!
                    sep.anchoredPosition = deltaPos;
                }
                deltaPos += step;
            }
        }
    }

    [System.Serializable]
    public class CtxMenuItem<T> : System.Tuple<string, Sprite, System.Action<T>>
    {
        public CtxMenuItem(string item1, Sprite item2, System.Action<T> item3) : base(item1, item2, item3)
        {
        }
    }
}

