#if UNITY_EDITOR
namespace A.Dialogue.Editor
{
    public class DialogueNode : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;
        public bool entryPoint;

        public NodeData nodeData;

        public DialogueNode()
        {

        }
    }
}
#endif
