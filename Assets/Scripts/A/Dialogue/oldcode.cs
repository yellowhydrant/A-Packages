//namespace A.Dialogue
//{
//    [AddComponentMenu(AConstants.ComponentMenuRoot + "/" + ADialogueConstants.ComponentMenuRoot + "/" + "Dialogue Parser")]
//    //[RequireComponent(typeof(PlayerInput))]
//    public class ADialogueParser : MonoBehaviour
//    {
//        [SerializeField] TMP_Text nameText;
//        [SerializeField] TMP_Text dialogueText;
//        [SerializeField] Image spriteImage;

//        [SerializeField] AButton choiceButtonPrefab;
//        [SerializeField] Transform choiceButtonContainer;

//        [SerializeField] AButton continueButton;
//        [SerializeField] GameObject backgroundBlur;

//        ADialogueGraph graph;
//        //To be changed
//        //TODO: Add timeline variable to nodes and play the timeline if it exists
//        //TODO: While timeline is plating disable parser
//        //TODO: Change on proceed and on end event to also take in graph
//        Action<string> onGraphEnd;
//        Action<string> onProceed;

//        bool canContinue;

//        public const string End = "End Dialogue";

//        private void OnEnable()
//        {
//            Cursor.lockState = CursorLockMode.Locked;
//        }

//        private void OnDisable()
//        {
//            Cursor.lockState = CursorLockMode.None;
//        }

//        public void StartDialogue(ADialogueGraph graph, Action<string> onGraphEnd, Action<string> onProceed)
//        {
//            foreach (Transform child in choiceButtonContainer)
//                Destroy(child.gameObject);
//            ActivateContinueButton(false);
//            if (backgroundBlur != null)
//                backgroundBlur.SetActive(false);

//            gameObject.SetActive(true);

//            this.onGraphEnd = onGraphEnd;
//            this.onProceed = onProceed;
//            this.graph = graph;

//            var entryNode = graph.nodeLinks.First();
//            ProceedToNextNode(entryNode.targetGUID);
//        }

//        public void EndDialogue(string guid)
//        {
//            if (!string.IsNullOrEmpty(guid))
//                onGraphEnd?.Invoke(guid);
//            gameObject.SetActive(false);
//        }

//        //void OnSubmit(InputValue value)
//        //{
//        //    if (value.isPressed && canContinue)
//        //    {
//        //        continueButton.onClick.Invoke();
//        //    }
//        //}

//        void InvokeNodeEvent(ANodeData nodeData, string nextNodeGUID)
//        {
//            // Change this to work with behaviour trees
//            onProceed.Invoke(nodeData.GUID);
//            ProceedToNextNode(nextNodeGUID);
//        }

//        private void ProceedToNextNode(string dialogueNodeGUID)
//        {
//            var node = graph.nodeData.Find(x => x.GUID == dialogueNodeGUID);

//            var choices = graph.nodeLinks.Where(x => x.baseGUID == dialogueNodeGUID).ToArray();
//            if (node.isBranch)
//            {
//                var max = 0;
//                for (int i = 0; i < choices.Length; i++)
//                {
//                    if (int.TryParse(choices[i].portName, out int result))
//                        max += result;
//                }
//                for (int i = 0; i < choices.Length; i++)
//                {
//                    int.TryParse(choices[i].portName, out int result);
//                    if (result <= Random.Range(0f, max))
//                        ProceedToNextNode(choices[i].targetGUID);
//                }
//                ProceedToNextNode(choices[Random.Range(0, choices.Length)].targetGUID);
//            }

//            //Speaker setup
//            if (nameText != null)
//                nameText.text = node.speaker.name;
//            if (spriteImage != null)
//                spriteImage.sprite = node.speaker.sprite;

//            //StopCoroutine(nameof(TypeDialogue));
//            //StartCoroutine(TypeDialogue(node));
//            dialogueText.text = ProcessProperties(node.speakerDialogue);

//            foreach (Transform child in choiceButtonContainer)
//                Destroy(child.gameObject);
//            if (backgroundBlur != null)
//                backgroundBlur.SetActive(false);

//            if (choices.Length == 1)
//            {
//                if (choices[0].portName.ToLower() == ADialogueGraph.Continue.ToLower())
//                {
//                    continueButton.onClick.RemoveAllListeners();
//                    continueButton.onClick.AddListener(() =>
//                    {
//                        ActivateContinueButton(false);
//                        InvokeNodeEvent(node, choices[0].targetGUID);
//                    });
//                    ActivateContinueButton(true);
//                    return;
//                }
//            }

//            if (choices == null || choices.Length == 0)
//            {
//                continueButton.onClick.RemoveAllListeners();
//                continueButton.onClick.AddListener(() =>
//                {
//                    EndDialogue(dialogueNodeGUID);
//                    continueButton.mainText.text = ADialogueGraph.Continue;
//                });
//                ActivateContinueButton(true);
//                continueButton.mainText.text = End;
//                return;
//            }

//            if (backgroundBlur != null)
//                backgroundBlur.SetActive(true);

//            var offset = new Vector2(0, choiceButtonPrefab.targetGraphic.rectTransform.sizeDelta.y);
//            var curOffset = new Vector2();
//            foreach (var choice in choices)
//            {
//                var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
//                button.targetGraphic.rectTransform.anchoredPosition = button.targetGraphic.rectTransform.anchoredPosition + curOffset;
//                button.mainText.text = ProcessProperties(choice.portName);
//                button.onClick.AddListener(() => InvokeNodeEvent(node, choice.targetGUID));
//                curOffset += offset;
//            }
//        }

//        //IEnumerator TypeDialogue(NodeData nodeData)
//        //{
//        //    dialogueText.text = string.Empty;
//        //    var fullDialogue = ProcessProperties(nodeData.speakerDialogue);
//        //    var currentDialogue = string.Empty;
//        //    for (int i = 0; i < fullDialogue.Length; i++)
//        //    {
//        //        currentDialogue += fullDialogue[i];
//        //        if (!string.IsNullOrEmpty(nodeData.styleTag))
//        //            dialogueText.text = $"<{nodeData.styleTag}>{currentDialogue}</{nodeData.styleTag}>";
//        //        else
//        //            dialogueText.text = currentDialogue;
//        //        yield return null;
//        //    }
//        //}

//        protected string ProcessProperties(string text)
//        {
//            foreach (var exposedProperty in graph.exposedProperties)
//            {
//                if (!exposedProperty.isTag)
//                {
//                    text = text.Replace("{" + exposedProperty.propertyName + "}", exposedProperty.propertyValue);
//                }
//                else
//                {
//                    text = text.Replace("<" + exposedProperty.propertyName + ">", $"<{exposedProperty.propertyValue}>");
//                    text = text.Replace("</" + exposedProperty.propertyName + ">", $"</{exposedProperty.propertyValue}>");
//                }
//            }
//            return text;
//        }

//        protected void ActivateContinueButton(bool isActive)
//        {
//            continueButton.interactable = isActive;
//            continueButton.mainText.gameObject.SetActive(isActive);
//            canContinue = isActive;
//        }
//    }
//}