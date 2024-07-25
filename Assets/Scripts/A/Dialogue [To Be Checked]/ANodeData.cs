using UnityEngine;

namespace A.Dialogue
{
    public abstract class ANodeData : ScriptableObject
    {
        public ADialogueActor actor
        {
            get => actor_ == null ? ADialogueActor.NullSpeaker : actor_;
            set => actor_ = value;
        }
        [SerializeField] ADialogueActor actor_;


        [HideInInspector]
        public Vector2 nodePosition;
        [HideInInspector]
        public string GUID;
        [HideInInspector]
        public ADialogueGraph.LinkData[] choices;

        //public virtual bool allowMultipleChoices { get; } = true;
        public virtual bool floatChoiceNames { get; } = false;
        public virtual System.Type portType { get; } = typeof(int);

        public void Init(string guid, Vector2 pos)
        {
            GUID = guid;
            nodePosition = pos;
        }

        public void Init(ANodeData nodeData)
        {
            GUID = nodeData.GUID;
            nodePosition = nodeData.nodePosition;
            actor = nodeData.actor;
        }

        public abstract void OnNodeEnter(ADialogueParser parser);
        public abstract void OnNodeUpdate(ADialogueParser parser);
        public abstract void OnNodeExit(ADialogueParser parser);
    }
}
