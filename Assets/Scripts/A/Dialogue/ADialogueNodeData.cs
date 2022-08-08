using UnityEngine;

namespace A.Dialogue
{
    public class ADialogueNodeData : ANodeData
    {
        public string actorDialogue;
        [SerializeField] ADialogueParser.TextAnimationType textAnimationType;
        [SerializeField] float choiceDelay = 0.4f;
        [SerializeField] AnimationCurve curve;
        [SerializeField] float typeDelay = .015f;

        public override System.Type portType => typeof(Vector2);

        public void Init(string guid, string dialogue, Vector2 pos)
        {
            base.Init(guid, pos);
            actorDialogue = dialogue;
        }

        public override void OnNodeEnter(ADialogueParser parser)
        {
            parser.RefreshActorUI(actor);
            parser.StartCoroutineWithCallback(parser.TypeDialogueCo(textAnimationType, this, curve, typeDelay), () => parser.StartCoroutine(parser.ShowChoiceButtons(choices, choiceDelay)));
        }

        public override void OnNodeUpdate(ADialogueParser parser)
        {
            // do nothing
        }

        public override void OnNodeExit(ADialogueParser parser)
        {
            // do nothing
        }
    }
}
