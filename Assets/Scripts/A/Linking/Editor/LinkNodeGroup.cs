#if UNITY_EDITOR
namespace A.Linking.Editor
{
    public class LinkNodeGroup : UnityEditor.Experimental.GraphView.Group
    {
        public string GUID;
        public ILinkable linkable;
    }
}
#endif