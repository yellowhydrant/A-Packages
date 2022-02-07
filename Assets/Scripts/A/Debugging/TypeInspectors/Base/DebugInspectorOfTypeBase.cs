using UnityEngine.UIElements;

namespace A.Debugging
{
    public abstract class DebugInspectorOfTypeBase
    {
        public object target { get; }
        public DebugInspectorOfTypeBase(object target)
        {
            this.target = target;
        }
        public abstract VisualElement CreateGUI();
        public abstract void UpdateGUI();
    }
}
