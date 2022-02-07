using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class VectorInspector<T> : DebugFieldInspectorOfType<T> where T : struct, IEquatable<T>, IFormattable
    {
        TextField[] textFields;
        int vectorSize { get; }

        const string letters = "XYZW";
        bool isInt { get; } = typeof(T) == typeof(Vector2Int) || typeof(T) == typeof(Vector3Int);

        public VectorInspector(object target, FieldInfo info) : base(target, info)
        {
            vectorSize = int.Parse(nameof(T).ToLower().Replace("int", null)[nameof(T).Length - 1].ToString());
        }

        public override VisualElement CreateGUI()
        {
            var container = new VisualElement();
            container.AddToClassList("field");
            container.Add(new Label(field.Name));

            var value = (Vector4)field.GetValue(target);
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
                    int resInt = 0;
                    float resFloat = 0;
                    if (isInt ? int.TryParse(newValue, out resInt) : float.TryParse(newValue, out resFloat))
                        value[index] = isInt ? resInt : resFloat;
                    else
                        textField.SetValueWithoutNotify(ctx.previousValue);
                    // change to only change the xyzw variables of the vector
                    field.SetValue(target, typeof(T) == typeof(Vector4) ? value : typeof(T).GetMethod("op_Implicit", new Type[] { typeof(T) }).Invoke(null, new object[] { value }));
                });
            }

            return container;
        }

        public override void UpdateGUI()
        {
            var value = (Vector4)field.GetValue(target);
            for (int i = 0; i < textFields.Length; i++)
            {
                if(textFields[i].panel.focusController.focusedElement != textFields[i])
                    textFields[i].SetValueWithoutNotify(value[i].ToString());
            }
        }
    }
}
