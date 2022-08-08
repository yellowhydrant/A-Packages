using System.Collections;
using System.Collections.Generic;
using System.Linq;
using A.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace A.Dialogue
{
    public class ADialogueParser : MonoBehaviour
    {
        [field: SerializeField]
        public ADialogueGraph graph { get; protected set; }

        [SerializeField] TMP_Text actorName;
        [SerializeField] TMP_Text actorDialogue;
        [SerializeField] Image actorImage;

        [SerializeField] AButton choiceButtonPrefab;
        [SerializeField] Transform choiceButtonContainer;

        [SerializeField] AButton gotoNextButton;
        [SerializeField] string EndDialogueButtonText = "End Dialogue";
        [SerializeField] string ContinueDialogueButtonText = "Continue";

        [SerializeField] GameObject backgroundBlur;

        PlayableDirector director;

        ANodeData currentNode;
        
        public enum TextAnimationType
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
            if (Application.isPlaying)
                StartDialogue(testGraph);
        }

        #region Parser side
        public void StartDialogue(ADialogueGraph graph)
        {
            this.graph = graph;

            var entryLink = graph.nodeLinks.First();
            currentNode = graph.nodeData.Find((x) => x.GUID == entryLink.targetGUID);

            currentNode.OnNodeEnter(this);
            currentNode.choices = graph.nodeLinks.Where((x) => x.baseGUID == currentNode.GUID).ToArray();

            ResetAll();
        }

        private void Update()
        {
            if(graph != null && currentNode != null)
                currentNode.OnNodeUpdate(this);
        }

        public void ContinueDialogue(ADialogueGraph.LinkData linkData)
        {
            currentNode.OnNodeExit(this);

            if (linkData != null)
            {
                currentNode = graph.nodeData.Find((x) => x.GUID == linkData.targetGUID);
                currentNode.OnNodeEnter(this);
                currentNode.choices = graph.nodeLinks.Where((x) => x.baseGUID == currentNode.GUID).ToArray();
            }
            else
            {
                Debug.Log("End Of Dialogue");
            }
        }
        #endregion

        public void StartCoroutineWithCallback(IEnumerator coroutine, System.Action onCoroutineEnd)
        {
            StartCoroutine(StartCoroutineWithCallbackCo(coroutine, onCoroutineEnd));
        }

        IEnumerator StartCoroutineWithCallbackCo(IEnumerator coroutine, System.Action onCoroutineEnd)
        {
            yield return coroutine;
            onCoroutineEnd.Invoke();
        }

        public void RefreshActorUI(ADialogueActor actor)
        {
            if(actorName != null)
                actorName.text = actor.name;
            if(actorImage != null)
                actorImage.sprite = actor.sprite;
        }

        public void HideAllUI(bool state)
        {

        }

        public IEnumerator ShowChoiceButtons(ADialogueGraph.LinkData[] choices, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (choices.Length == 0 || (choices.Length == 1 && choices[0].portName == ADialogueGraph.GotoNext))
                ShowGoToNextButton(choices.Length == 0 ? null : choices[0], choices.Length == 0);
            else
                InstantiateChoiceButtons(choices);
        }

        public void PlayTimeline(TimelineAsset timeline, MyBox.MyDictionary<Object, Object> bindings)
        {
            if (director == null)
                director = gameObject.AddComponent<PlayableDirector>();
            director.playableAsset = timeline;
            foreach(var pair in bindings)
            {
                director.SetGenericBinding(pair.Key, pair.Value);
            }
        }

        #region old
        //IEnumerator ContinueDialogueCo(ADialogueGraph.LinkData link)
        //{
        //    //Get node
        //    var node = graph.nodeData.Find((x) => x.GUID == link.targetGUID);

        //    //Reset all fields
        //    ResetAll();

        //    //Wait until can resume dialogue for cutscenes etc
        //    while (!onBeforeContinue.Invoke(link))
        //        yield return null;

        //    //Get all choices
        //    var choices = graph.nodeLinks.Where((x) => x.baseGUID == node.GUID).ToArray();
        //    var isEndNode = choices == null || choices.Length == 0;

        //    //Assign actor name and sprite
        //    if (actorName != null)
        //        actorName.text = node.actor.name;
        //    if (actorImage != null)
        //        actorImage.sprite = node.actor.sprite;

        //    //Type out dialogue with animation
        //    yield return TypeDialogueCo(node as ADialogueNodeData);

        //    //If there is only one choice or its the last node show the continue/end text and click on text body to proceed
        //    if (isEndNode || choices.Length == 1)
        //    {
        //        gotoNextButton.onClick.RemoveAllListeners();

        //        if (isEndNode)
        //            gotoNextButton.onClick.AddListener(() => 
        //            {
        //                EndDialogue(node);
        //            });
        //        else if(choices[0].portName.ToLower() == ADialogueGraph.GotoNext.ToLower())
        //            gotoNextButton.onClick.AddListener(() =>
        //            {
        //                ContinueDialogue(choices[0]);
        //            });

        //        gotoNextButton.onClick.AddListener(() =>
        //        {
        //            SetActiveGotoNextButton(false, isEndNode);
        //        });
        //        SetActiveGotoNextButton(true, isEndNode);
        //        yield break;
        //    }

        //    yield return new WaitForSeconds(choiceDelay);

        //    if (backgroundBlur)
        //        backgroundBlur.SetActive(true);

        //    InstantiateChoiceButtons(choices);
        //}
        #endregion

        protected virtual void InstantiateChoiceButtons(ADialogueGraph.LinkData[] choices)
        {
            Debug.Log("gotonext");
            var offset = new Vector2(0, choiceButtonPrefab.rect.sizeDelta.y);
            var curOffset = new Vector2();
            for (int i = 0; i < choices.Length; i++)
            {
                var button = Instantiate(choiceButtonPrefab, choiceButtonContainer);
                button.rect.anchoredPosition = button.rect.anchoredPosition + curOffset;
                button.mainText.text = ProcessProperties(choices[i].portName);
                var choice = choices[i];
                button.onClick.AddListener(() => { ContinueDialogue(choice); ResetAll(); });
                curOffset += offset;
            }
        }

        //public void ContinueDialogue(ADialogueGraph.LinkData link)
        //{
        //    //StopCoroutine(nameof(ContinueDialogueCo));
        //    StopAllCoroutines();

        //    StartCoroutine(ContinueDialogueCo(link));
        //}

        //public void EndDialogue(ANodeData endNode)
        //{
        //    onEnd.Invoke(endNode);
        //}

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
        #region TextAnimation
        public IEnumerator TypeDialogueCo(TextAnimationType animationType, ADialogueNodeData node, AnimationCurve curve, float typeDelay)
        {
            switch (animationType)
            {
                case TextAnimationType.InOneGo:
                    yield return TypeDialogueFullyCo(node);
                    break;
                case TextAnimationType.TypeLetterByLetter:
                    yield return TypeDialogueLetterByLetterCo(node, typeDelay);
                    break;
                case TextAnimationType.RevealLetterByLetter:
                    yield return RevealDialogueLetterByLetterCo(node, typeDelay);
                    break;
                case TextAnimationType.TypeWordByWord:
                    yield return TypeDialogueWordByWordCo(node, typeDelay);
                    break;
                case TextAnimationType.TypeWordByWordAnimated:
                    yield return TypeDialogueWordByWordCo(node, curve, typeDelay);
                    break;
                case TextAnimationType.RevealWordByWord:
                    yield return RevealDialogueWordByWordCo(node, curve, typeDelay);
                    break;
            }
        }

        public IEnumerator TypeDialogueLetterByLetterCo(ADialogueNodeData node, float typeDelay)
        {
            var delay = new WaitForSeconds(typeDelay);
            var original = ProcessProperties(node.actorDialogue);
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

        public IEnumerator RevealDialogueLetterByLetterCo(ADialogueNodeData node, float typeDelay)
        {
            var delay = new WaitForSeconds(typeDelay);
            var original = ProcessProperties(node.actorDialogue);
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

        public IEnumerator TypeDialogueWordByWordCo(ADialogueNodeData node, float typeDelay)
        {
            var delay = new WaitForSeconds(typeDelay);
            var original = ProcessProperties(node.actorDialogue);
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

        public IEnumerator TypeDialogueWordByWordCo(ADialogueNodeData node, AnimationCurve curve, float typeDelay)
        {
            if (curve == null)
                curve = AnimationCurve.Linear(0, 0, 1, 1);

            var curDelay = 0f;
            var original = ProcessProperties(node.actorDialogue);
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

        public IEnumerator RevealDialogueWordByWordCo(ADialogueNodeData node, AnimationCurve curve, float typeDelay)
        {
            if (curve == null)
                curve = AnimationCurve.Linear(0, 0, 1, 1);

            var original = ProcessProperties(node.actorDialogue);
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

        public IEnumerator TypeDialogueFullyCo(ADialogueNodeData node)
        {
            actorDialogue.text = ProcessProperties(node.actorDialogue);
            yield break;
        }
        #endregion

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

        public void ShowGoToNextButton(ADialogueGraph.LinkData linkData, bool endOfDialogue)
        {
            SetActiveGotoNextButton(true, endOfDialogue);

            gotoNextButton.onClick.RemoveAllListeners();
            gotoNextButton.onClick.AddListener(() => 
            {
                ContinueDialogue(linkData);
                ResetAll();
            });
        }

        protected virtual void SetActiveGotoNextButton(bool state, bool endOfDialogue)
        {
            gotoNextButton.interactable = state;
            gotoNextButton.mainText.gameObject.SetActive(state);
            if(gotoNextButton.mainText != null)
                gotoNextButton.mainText.text = endOfDialogue ? EndDialogueButtonText : ContinueDialogueButtonText;
        }
    }
}