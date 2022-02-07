using UnityEngine;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class TransformInspector : DebugComponentInspectorOfType<Transform>
    {
        public TransformInspector(object target) : base(target)
        {
        }

        public override VisualElement CreateGUI()
        {
            return null;
        }

        public override void UpdateGUI()
        {
            
        }
    }
}