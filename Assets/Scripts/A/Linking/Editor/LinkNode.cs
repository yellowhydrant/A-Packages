#if UNITY_EDITOR
namespace A.Linking.Editor
{
    public class LinkNode : UnityEditor.Experimental.GraphView.Node
    {
        public LinkNodeGroup group;
        public ILinkable.Link link;

        public LinkNode() : base()
        {

        }
    }
}
#endif