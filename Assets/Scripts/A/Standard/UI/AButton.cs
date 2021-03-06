using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace A.UI
{
    [AddComponentMenu("A/UI/Button")]
    public class AButton : Button
    {
        public TMP_Text mainText;
        public RectTransform rect => targetGraphic.rectTransform;

        protected override void Awake()
        {
            base.Awake();
            if(mainText == null)
                mainText = GetComponentInChildren<TMP_Text>();
        }
    }
}
