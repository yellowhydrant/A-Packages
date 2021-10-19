using UnityEngine;

namespace A.Dialogue
{
    public class NodeData : ScriptableObject
    {
        [Header("Main")]
        public Sprite speakerSprite;
        public string speakerName;
        public string speakerDialogue;
        public string styleTag;

        public bool lol;

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
