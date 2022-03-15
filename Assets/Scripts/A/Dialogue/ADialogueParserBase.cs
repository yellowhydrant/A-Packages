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
    public class ADialogueParserBase : MonoBehaviour
    {
        public System.Action<ANodeData> onEnd;
        public System.Func<ADialogueGraph.LinkData, bool> onBeforeContinue;
        public System.Action onAfterContinue;
        public TextAppearance textAppearance = TextAppearance.TypeLetterByLetter;

        [SerializeField] TMP_Text actorName;
        [SerializeField] TMP_Text actorDialogue;
        [SerializeField] Image actorImage;

        [SerializeField] AButton choiceButtonPrefab;
        [SerializeField] Transform choiceButtonContainer;

        [SerializeField] AButton gotoNextButton;
        [SerializeField] string EndDialogueButtonText = "End Dialogue";
        [SerializeField] string ContinueDialogueButtonText = "Continue";

        [SerializeField] GameObject backgroundBlur;

        [SerializeField, Range(0f, 1f)] float typeDelay;
        //TODO: Scale with text size
        [SerializeField, Range(0f, 2f)] float choiceDelay = 1.8f;

        ADialogueGraph graph;
        
        public enum TextAppearance
        {
            InOneGo,
            TypeLetterByLetter,
            RevealLetterByLetter,
            TypeWordByWord,
            TypeWordByWordAnimated,
            RevealWordByWord
        }

        [SerializeField] ADialogueGraph testGraph;

        [MyBox.ButtonMethod]
        void BeginTestDialogue()
        {
            if(Application.isPlaying)
                StartDialogue(testGraph, null, null);
        }


        public void StartDialogue(ADialogueGraph graph, System.Action<ANodeData> onEnd, System.Func<ADialogueGraph.LinkData, bool> onBeforeContinue)
        {
            if (onBeforeContinue == null) onBeforeContinue += (aeg0) => true;
            onEnd += (arg0) => gameObject.SetActive(false);

            gameObject.SetActive(true);

            this.graph = graph;
            this.onEnd = onEnd;
            this.onBeforeContinue = onBeforeContinue;

            var entryLink = graph.nodeLinks.First();
            StartCoroutine(ContinueDialogueCo(entryLink));
        }

        IEnumerator ContinueDialogueCo(ADialogueGraph.LinkData link)
        {
            //Get node
            var node = graph.nodeData.Find((x) => x.GUID == link.targetGUID);

            //Reset all fields
            ResetAll();

            //Wait until can resume dialogue for cutscenes etc
            while (!node.isBranch && !onBeforeContinue.Invoke(link))
                yield return null;

            //Get all choices
            var choices = graph.nodeLinks.Where((x) => x.baseGUID == node.GUID).ToArray();
            var isEndNode = choices == null || choices.Length == 0;

            //Handle branch node (later will be changed to handling inside the node class)
            if (node.isBranch)
            {
                var max = choices.Sum((x) => float.Parse(x.portName));
                for (int i = 0; i < choices.Length; i++)
                {
                    if (float.Parse(choices[i].portName) <= Random.Range(0, max))
                    {
                        yield return ContinueDialogueCo(choices[i]);
                        yield break;
                    }
                }
                yield return ContinueDialogueCo(choices[Random.Range(0, choices.Length)]);
                yield break;
            }

            //Assign actor name and sprite
            if (actorName != null)
                actorName.text = node.speaker.name;
            if (actorImage != null)
                actorImage.sprite = node.speaker.sprite;

            //Type out dialogue with animation
            yield return TypeDialogueCo(node);

            //If there is only one choice or its the last node show the continue/end text and click on text body to proceed
            if (isEndNode || choices.Length == 1)
            {
                gotoNextButton.onClick.RemoveAllListeners();

                if (isEndNode)
                    gotoNextButton.onClick.AddListener(() => 
                    {
                        EndDialogue(node);
                    });
                else if(choices[0].portName.ToLower() == ADialogueGraph.GotoNext.ToLower())
                    gotoNextButton.onClick.AddListener(() =>
                    {
                        ContinueDialogue(choices[0]);
                    });

                gotoNextButton.onClick.AddListener(() =>
                {
                    SetActiveGotoNextButton(false, isEndNode);
                });
                SetActiveGotoNextButton(true, isEndNode);
                yield break;
            }

            yield return new WaitForSeconds(choiceDelay);

            if (backgroundBlur)
                backgroundBlur.SetActive(true);

            InstantiateChoiceButtons(choices);
        }

        protected virtual void InstantiateChoiceButtons(ADialogueGraph.LinkData[] choices)
        {
            var offset = new Vector2(0, choiceButtonPrefab.rect.sizeDelta.y);
            var curOffset = new Vector2();
            for (int i = 0; i < choices.Length; i++)
            {
                var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                button.rect.anchoredPosition = button.rect.anchoredPosition + curOffset;
                button.mainText.text = ProcessProperties(choices[i].portName);
                var choice = choices[i];
                button.onClick.AddListener(() => ContinueDialogue(choice));
                curOffset += offset;
            }
        }

        public void ContinueDialogue(ADialogueGraph.LinkData link)
        {
            //StopCoroutine(nameof(ContinueDialogueCo));
            StopAllCoroutines();

            StartCoroutine(ContinueDialogueCo(link));
        }

        public void EndDialogue(ANodeData endNode)
        {
            onEnd.Invoke(endNode);
        }

        //TODO: Remove tags from properties, add blackboard for actors and rename speaker to actor
        public string ProcessProperties(string text)
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

        protected IEnumerator TypeDialogueCo(ANodeData node)
        {
            switch (textAppearance)
            {
                case TextAppearance.InOneGo:
                    yield return TypeDialogueFullyCo(node);
                    break;
                case TextAppearance.TypeLetterByLetter:
                    yield return TypeDialogueLetterByLetterCo(node);
                    break;
                case TextAppearance.RevealLetterByLetter:
                    yield return RevealDialogueLetterByLetterCo(node);
                    break;
                case TextAppearance.TypeWordByWord:
                    yield return TypeDialogueWordByWordCo(node);
                    break;
                case TextAppearance.TypeWordByWordAnimated:
                    yield return TypeDialogueWordByWordCo(node, AnimationCurve.EaseInOut(0, 0, 1, 1));
                    break;
                case TextAppearance.RevealWordByWord:
                    yield return RevealDialogueWordByWordCo(node, AnimationCurve.EaseInOut(0, 0, 1, 1));
                    break;
            }
        }

        protected IEnumerator TypeDialogueLetterByLetterCo(ANodeData node)
        {
            var delay = new WaitForSeconds(typeDelay);
            var original = ProcessProperties(node.speakerDialogue);
            //Get all tag locations
            var tags = new List<System.Tuple<int, int, int, int>>();
            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] == '<' && i != original.Length - 1)
                {
                    if (original[i + 1] != '/')
                    {
                        var cur = i;
                        while (original[cur] != '>')
                            cur++;
                        tags.Add(new(i, cur, 0, 0));
                    }
                    else
                    {
                        var cur = i;
                        while (original[cur] != '>')
                            cur++;
                        tags[tags.Count - 1] = new(tags[tags.Count - 1].Item1, tags[tags.Count - 1].Item2, i, cur);
                    }
                }
            }

            //Type out text letter by letter with tags in mind
            var output = string.Empty;
            var postfix = string.Empty;
            var threshold = 0;
            for (int i = 0; i < original.Length; i++)
            {
                for (int j = 0; j < tags.Count; j++)
                {
                    if (tags[j].Item1 == i)
                    {
                        output += original.Substring(tags[j].Item1, tags[j].Item2 - tags[j].Item1 + 1);
                        i = tags[j].Item2 + 1;
                        threshold = tags[j].Item3 - 1;
                        postfix = original.Substring(tags[j].Item3, tags[j].Item4 - tags[j].Item3 + 1);
                        break;
                    }
                }
                output += original[i];
                if (i == threshold)
                {
                    output += postfix;
                    i = output.Length - 1;
                    postfix = null;
                }

                actorDialogue.text = output + postfix;
                if (typeDelay <= 0)
                    yield return null;
                else
                    yield return delay;
            }
        }

        protected IEnumerator RevealDialogueLetterByLetterCo(ANodeData node)
        {
            var delay = new WaitForSeconds(typeDelay);
            var original = ProcessProperties(node.speakerDialogue);
            //Get all tag locations
            var tags = new List<System.Tuple<int, int, int, int>>();
            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] == '<' && i != original.Length - 1)
                {
                    if (original[i + 1] != '/')
                    {
                        var cur = i;
                        while (original[cur] != '>')
                            cur++;
                        tags.Add(new(i, cur, 0, 0));
                    }
                    else
                    {
                        var cur = i;
                        while (original[cur] != '>')
                            cur++;
                        tags[tags.Count - 1] = new(tags[tags.Count - 1].Item1, tags[tags.Count - 1].Item2, i, cur);
                    }
                }
            }

            //Type out text letter by letter with tags in mind
            var output = string.Empty;
            var postfix = string.Empty;
            var threshold = 0;
            for (int i = 0; i < original.Length; i++)
            {
                for (int j = 0; j < tags.Count; j++)
                {
                    if (tags[j].Item1 == i)
                    {
                        output += original.Substring(tags[j].Item1, tags[j].Item2 - tags[j].Item1 + 1);
                        i = tags[j].Item2 + 1;
                        threshold = tags[j].Item3 - 1;
                        postfix = original.Substring(tags[j].Item3, tags[j].Item4 - tags[j].Item3 + 1);
                        break;
                    }
                }
                output += original[i];
                if (i == threshold)
                {
                    output += postfix;
                    i = output.Length - 1;
                    postfix = null;
                }

                var spacer = original.Substring(output.Length);
                spacer = System.Text.RegularExpressions.Regex.Replace(spacer, "<.*?>", string.Empty);
                spacer = "<alpha=#00>" + spacer;
                actorDialogue.text = (output + postfix) + spacer;

                if (typeDelay <= 0)
                    yield return null;
                else
                    yield return delay;
            }
        }

        //var spacer = string.Empty.PadRight(original.Length - (output.Length + postfix.Length), ' ');
        //actorDialogue.text = (output + postfix) + spacer;

        protected IEnumerator TypeDialogueWordByWordCo(ANodeData node)
        {
            var delay = new WaitForSeconds(typeDelay);
            var original = ProcessProperties(node.speakerDialogue);
            var output = string.Empty;
            foreach (var word in original.Split(' '))
            {
                output += " " + word;
                actorDialogue.text = output;
                if (typeDelay <= 0)
                    yield return null;
                else
                    yield return delay;
            }
        }

        protected IEnumerator TypeDialogueWordByWordCo(ANodeData node, AnimationCurve curve)
        {
            if (curve == null)
                curve = AnimationCurve.Linear(0, 0, 1, 1);

            var curDelay = 0f;
            var original = ProcessProperties(node.speakerDialogue);
            var output = string.Empty;
            foreach (var word in original.Split(' '))
            {
                curDelay = 0;
                while (curDelay < typeDelay)
                {
                    var hex = ColorUtility.ToHtmlStringRGBA(new Color(0, 0, 0, curve.Evaluate(curDelay / typeDelay))).Substring(6);
                    actorDialogue.text = $"{output} <alpha=#{hex}>{word}";
                    curDelay += Time.deltaTime;
                    yield return null;
                }
                output += " " + word;
            }
        }

        protected IEnumerator RevealDialogueWordByWordCo(ANodeData node, AnimationCurve curve)
        {
            if (curve == null)
                curve = AnimationCurve.Linear(0, 0, 1, 1);

            var original = ProcessProperties(node.speakerDialogue);
            var spaces = new List<int>();
            for (int i = 0; i < original.Length; i++)
            {
                if (original[i] == ' ')
                    spaces.Add(i);
            }

            var curDelay = 0f;
            for (int i = 0; i < spaces.Count; i++)
            {
                curDelay = 0;
                while (curDelay < typeDelay)
                {
                    var hex = ColorUtility.ToHtmlStringRGBA(new Color(0, 0, 0, curve.Evaluate(curDelay / typeDelay))).Substring(6);
                    var tag = $"<alpha=#{hex}>";

                    var output = original.Insert(spaces[i], tag);
                    if (i != spaces.Count - 1)
                        output = output.Insert(spaces[i + 1] + tag.Length, "<alpha=#00>");
                    actorDialogue.text = output;

                    curDelay += Time.deltaTime;
                    yield return null;
                }
            }
        }

        protected IEnumerator TypeDialogueFullyCo(ANodeData node)
        {
            actorDialogue.text = ProcessProperties(node.speakerDialogue);
            yield break;
        }

        protected virtual void ResetAll()
        {
            ClearButtonContaier();
            SetActiveGotoNextButton(false, false);
            if(backgroundBlur)
                backgroundBlur.SetActive(false);
        }

        protected void ClearButtonContaier()
        {
            foreach (Transform child in choiceButtonContainer)
                Destroy(child.gameObject);
        }

        protected virtual void SetActiveGotoNextButton(bool state, bool endOfDialogue)
        {
            gotoNextButton.interactable = state;
            gotoNextButton.mainText.gameObject.SetActive(state);
            if(gotoNextButton.mainText)
                gotoNextButton.mainText.text = endOfDialogue ? EndDialogueButtonText : ContinueDialogueButtonText;
        }
    }
}

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