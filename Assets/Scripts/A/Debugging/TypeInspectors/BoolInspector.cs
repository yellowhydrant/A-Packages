using UnityEngine.UIElements;

namespace A.Debugging
{
    public class BoolInspector : DebugFieldInspectorOfType<bool>
    {
        Toggle toggle;

        public BoolInspector(object target, System.Reflection.FieldInfo info) : base(target, info)
        {
        }

        public override VisualElement CreateGUI()
        {
            toggle = new Toggle { label = field.Name };
            toggle.SetValueWithoutNotify(System.Convert.ToBoolean(field.GetValue(target)));
            toggle.RegisterValueChangedCallback((ctx) => 
            {
                field.SetValue(target, ctx.newValue);
            });
            return toggle;
        }

        public override void UpdateGUI()
        {
            if (toggle.panel.focusController.focusedElement != toggle)
                toggle.SetValueWithoutNotify(System.Convert.ToBoolean(field.GetValue(target)));
        }
    }
}