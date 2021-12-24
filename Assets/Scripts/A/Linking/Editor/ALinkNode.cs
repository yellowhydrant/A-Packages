#if UNITY_EDITOR
namespace A.Linking.Editor
{
    public class ALinkNode : UnityEditor.Experimental.GraphView.Node
    {
        public ALinkNodeGroup group;
        public ILinkable.Link link;

        public ALinkNode() : base()
        {

        }
    }
}
#endif