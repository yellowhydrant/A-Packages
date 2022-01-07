#if UNITY_EDITOR
using A.BehaviourTree.Nodes;
using UnityEditor;

namespace A.BehaviourTree
{
    [CustomEditor(typeof(ANode), true)]
    public class ANodeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject, "m_Script", "child", "children");
        }
    }
}
#endif