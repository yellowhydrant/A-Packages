using System;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class NumberInspector<T> : DebugFieldInspectorOfType<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        TextField textField;

        public NumberInspector(object target, System.Reflection.FieldInfo info) : base(target, info)
        {
        }

        public override VisualElement CreateGUI()
        {
            textField = new TextField { label = field.Name};
            textField.SetValueWithoutNotify(field.GetValue(target).ToString());
            textField.RegisterValueChangedCallback((ctx) =>
            {
                try
                {
                    field.SetValue(target, System.Convert.ChangeType(ctx.newValue, typeof(T)));
                }
                catch
                {
                    textField.SetValueWithoutNotify(ctx.previousValue);
                }
            });
            return textField;
        }

        public override void UpdateGUI()
        {
            if(textField.panel.focusController.focusedElement != textField)
                textField.SetValueWithoutNotify(field.GetValue(target).ToString());
        }
    }
}