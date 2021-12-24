using UnityEngine;

namespace A.Dialogue
{
    public class ANodeData : ScriptableObject
    {
        public ADialogueSpeaker speaker
        {
            get => speaker_ == null ? ADialogueSpeaker.NullSpeaker : speaker_;
            set => speaker_ = value;
        }
        [SerializeField] ADialogueSpeaker speaker_;
        public string speakerDialogue;

        [HideInInspector]
        public Vector2 nodePosition;
        [HideInInspector]
        public string GUID;

        public void Init(string guid, string dialogue, Vector2 pos)
        {
            GUID = guid;
            speakerDialogue = dialogue;
            nodePosition = pos;
        }
    }
}
