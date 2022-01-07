//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.Events;

//namespace A.UI
//{
//    public class AContextualMenu : MonoBehaviour
//    {
//        public System.Action menu;

//        GameObject textPrefab;
//        GameObject buttonPrefab;
//        GameObject separatorPrefab;
//        GameObject whiteSpacePrefab;

//        private void Awake()
//        {
//            //Load stuff from resources
//        }

//        public void AppendText(string text)
//        {
//            var textObj = Instantiate(textPrefab, transform).GetComponentInChildren<TMP_Text>();
//            textObj.text = text;
//        }

//        public void AppendAction(string text, UnityAction action)
//        {
//            var buttonObj = Instantiate(buttonPrefab, transform).GetComponentInChildren<AButton>();
//            buttonObj.mainText.text = text;
//            buttonObj.onClick.AddListener(action);
//        }

//        public void AppendSeparator()
//        {
//            Instantiate(separatorPrefab, transform);
//        }

//        public void AppendWhiteSpace()
//        {
//            Instantiate(whiteSpacePrefab, transform);
//        }

//        public void BuildContextualMenu()
//        {
//            foreach (Transform child in transform)
//            {
//                Destroy(child.gameObject);
//            }
//            menu.Invoke();
//        }
//    }
//}
