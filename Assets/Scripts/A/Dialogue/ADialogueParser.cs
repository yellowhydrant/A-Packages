using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using A.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace A.Dialogue
{
    [RequireComponent(typeof(PlayerInput))]
    public class ADialogueParser : MonoBehaviour
    {
        [SerializeField] TMP_Text nameText;
        [SerializeField] TMP_Text dialogueText;

        [SerializeField] AButton choiceButtonPrefab;
        [SerializeField] Transform choiceButtonContainer;

        [SerializeField] AButton continueButton;
        [SerializeField] GameObject backgroundBlur;

        ADialogueGraph graph;
        Action<string> onGraphEnd;
        Action<string> onProceed;

        bool canContinue;

        public const string Continue = "Continue";
        public const string End = "End Dialogue";

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public void StartDialogue(ADialogueGraph graph, Action<string> onGraphEnd, Action<string> onProceed)
        {
            foreach (Transform child in choiceButtonContainer)
                Destroy(child.gameObject);
            ActivateContinueButton(false);
            if (backgroundBlur != null)
                backgroundBlur.SetActive(false);

            gameObject.SetActive(true);

            this.onGraphEnd = onGraphEnd;
            this.onProceed = onProceed;
            this.graph = graph;

            var entryNode = graph.nodeLinks.First();
            ProceedToNextNode(entryNode.targetGUID);
        }

        public void EndDialogue(string guid)
        {
            if (!string.IsNullOrEmpty(guid))
                onGraphEnd?.Invoke(guid);
            gameObject.SetActive(false);
        }

        void OnSubmit(InputValue value)
        {
            if (value.isPressed && canContinue)
            {
                continueButton.onClick.Invoke();
            }
        }

        void InvokeNodeEvent(NodeData nodeData, string nextNodeGUID)
        {
            // Change this to work with behaviour trees
            onProceed.Invoke(nodeData.GUID);
            ProceedToNextNode(nextNodeGUID);
        }

        private void ProceedToNextNode(string dialogueNodeGUID)
        {
            var node = graph.nodeData.Find(x => x.GUID == dialogueNodeGUID);
            nameText.text = node.speakerName;

            var choices = graph.nodeLinks.Where(x => x.baseGUID == dialogueNodeGUID);

            //StopCoroutine(nameof(TypeDialogue));
            //StartCoroutine(TypeDialogue(node));
            dialogueText.text = ProcessProperties(node.speakerDialogue);

            foreach (Transform child in choiceButtonContainer)
                Destroy(child.gameObject);
            if (backgroundBlur != null)
                backgroundBlur.SetActive(false);

            var choiceArray = choices.ToArray();
            if (choiceArray.Length == 1)
            {
                if (choiceArray[0].portName.ToLower() == Continue.ToLower())
                {
                    continueButton.onClick.RemoveAllListeners();
                    continueButton.onClick.AddListener(() =>
                    {
                        ActivateContinueButton(false);
                        InvokeNodeEvent(node, choiceArray[0].targetGUID);
                    });
                    ActivateContinueButton(true);
                    return;
                }
            }

            if (choiceArray == null || choiceArray.Length == 0)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() =>
                {
                    EndDialogue(dialogueNodeGUID);
                    continueButton.text.text = Continue;
                });
                ActivateContinueButton(true);
                continueButton.text.text = End;
                return;
            }

            if (backgroundBlur != null)
                backgroundBlur.SetActive(true);

            var offset = new Vector2(0, choiceButtonPrefab.targetGraphic.rectTransform.sizeDelta.y);
            var curOffset = new Vector2();
            foreach (var choice in choices)
            {
                var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                button.targetGraphic.rectTransform.anchoredPosition = button.targetGraphic.rectTransform.anchoredPosition + curOffset;
                button.text.text = ProcessProperties(choice.portName);
                button.onClick.AddListener(() => InvokeNodeEvent(node, choice.targetGUID));
                curOffset += offset;
            }
        }

        //IEnumerator TypeDialogue(NodeData nodeData)
        //{
        //    dialogueText.text = string.Empty;
        //    var fullDialogue = ProcessProperties(nodeData.speakerDialogue);
        //    var currentDialogue = string.Empty;
        //    for (int i = 0; i < fullDialogue.Length; i++)
        //    {
        //        currentDialogue += fullDialogue[i];
        //        if (!string.IsNullOrEmpty(nodeData.styleTag))
        //            dialogueText.text = $"<{nodeData.styleTag}>{currentDialogue}</{nodeData.styleTag}>";
        //        else
        //            dialogueText.text = currentDialogue;
        //        yield return null;
        //    }
        //}

        protected string ProcessProperties(string text)
        {
            foreach (var exposedProperty in graph.exposedProperties)
            {
                if (!exposedProperty.isTag)
                {
                    text = text.Replace("{" + exposedProperty.propertyName + "}", exposedProperty.propertyValue);
                }
                else
                {
                    text = text.Replace("<" + exposedProperty.propertyName + ">", $"<{exposedProperty.propertyValue}>");
                    text = text.Replace("</" + exposedProperty.propertyName + ">", $"</{exposedProperty.propertyValue}>");
                }
            }
            return text;
        }

        protected void ActivateContinueButton(bool isActive)
        {
            continueButton.interactable = isActive;
            continueButton.text.gameObject.SetActive(isActive);
            canContinue = isActive;
        }
    }
}