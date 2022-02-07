using System;
using System.Reflection;

namespace A.Debugging
{
    public abstract class DebugComponentInspectorOfType<T> : DebugInspectorOfTypeBase where T : UnityEngine.Component
    {
        public DebugComponentInspectorOfType(object target) : base(target)
        {
        }
    }
}
