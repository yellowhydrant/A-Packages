#if UNITY_EDITOR
namespace A.Dialogue.Editor
{
    public class ADialogueNode : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;
        public bool entryPoint;

        public ANodeData nodeData;

        public ADialogueNode()
        {

        }
    }
}
#endif
