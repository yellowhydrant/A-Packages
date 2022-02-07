using UnityEngine.UIElements;

namespace A.Debugging
{
    public class StringInspector : DebugFieldInspectorOfType<string>
    {
        TextField textField;

        public StringInspector(object target, System.Reflection.FieldInfo info) : base(target, info)
        {

        }

        public override VisualElement CreateGUI()
        {
            textField = new TextField { label = field.Name };
            textField.SetValueWithoutNotify(field.GetValue(target).ToString());
            textField.RegisterValueChangedCallback((ctx) =>
            {
                field.SetValue(target, ctx.newValue);
            });
            return textField;
        }

        public override void UpdateGUI()
        {
            if (textField.panel.focusController.focusedElement != textField)
                textField.SetValueWithoutNotify(field.GetValue(target).ToString());
        }
    }
}