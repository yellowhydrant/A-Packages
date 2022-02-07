using System.Reflection;

namespace A.Debugging
{
    public abstract class DebugFieldInspectorOfType<T> : DebugInspectorOfTypeBase
    {
        public FieldInfo field;

        public DebugFieldInspectorOfType(object target, FieldInfo info) : base(target)
        {
            field = info;
        }
    }
}