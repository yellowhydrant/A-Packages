using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class QuaternionInspector : DebugFieldInspectorOfType<Quaternion>
    {
        TextField[] textFields;
        int vectorSize { get; } = 4;

        const string letters = "XYZW";

        public QuaternionInspector(object target, FieldInfo info) : base(target, info)
        {
        }

        public override VisualElement CreateGUI()
        {
            var container = new VisualElement();
            container.AddToClassList("field");
            container.Add(new Label(field.Name));

            var value = (Quaternion)field.GetValue(target);
            textFields = new TextField[vectorSize];
            for (int i = 0; i < vectorSize; i++)
            {
                var textField = new TextField();
                var flabel = new Label(letters[i].ToString());
                textField.Insert(0, flabel);
                textField.style.width = Length.Percent((100f - 20f) / vectorSize);
                container.Add(textField);
                textFields[i] = textField;

                var index = i;
                textField.SetValueWithoutNotify(value[i].ToString());
                textField.RegisterValueChangedCallback((ctx) =>
                {
                    string newValue = ctx.newValue;
                    if (string.IsNullOrEmpty(newValue) || string.IsNullOrWhiteSpace(newValue))
                        newValue = (0).ToString();
                    if (float.TryParse(newValue, out float result))
                        value[index] = result;
                    else
                        textField.SetValueWithoutNotify(ctx.previousValue);
                    field.SetValue(target, value); // change to only change the xyzw variables of the quaternion
                });
            }

            return container;
        }

        public override void UpdateGUI()
        {
            var value = (Quaternion)field.GetValue(target);
            for (int i = 0; i < textFields.Length; i++)
            {
                if (textFields[i].panel.focusController.focusedElement != textFields[i])
                    textFields[i].SetValueWithoutNotify(value[i].ToString());
            }
        }
    }
}
