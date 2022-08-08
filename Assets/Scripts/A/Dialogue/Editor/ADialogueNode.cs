#if UNITY_EDITOR
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace A.Dialogue.Editor
{
    public class ADialogueNode : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;
        public bool entryPoint;

        public ANodeData nodeData;
        public UnityEditor.Editor editor;

        public ADialogueNode()
        {

        }
    }
}
#endif
