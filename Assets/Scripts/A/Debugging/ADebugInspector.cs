using A.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

//TODO:
//Make enum inspector
//Make flags inspector
//Make array inspector
//Add fast getter setter methods
//Test
//Make it so there is recursive serialization of non object types (with limit)
//Add field that just display texture objects
//Move on to hierarchy after test
//Move on to loaded objects after hierarchy
//Finish off with console

namespace A.Debugging
{
    public class ADebugInspector : MonoBehaviour
    {
        [SerializeField] GameObject target;

        VisualElement root;
        List<DebugInspectorOfTypeBase> inspectors;

        private void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            //SwitchTarget(target);
        }

        private void SwitchTarget(GameObject target)
        {
            var root = this.root;
            var components = target.GetComponents<Component>();
            var allTypes = typeof(DebugInspectorOfTypeBase).Assembly.ExportedTypes.Where((t) => t.IsSubclassOf(typeof(DebugInspectorOfTypeBase)));
            for (int c = 0; c < components.Length; c++)
            {
                var type = components[c].GetType();
                if (type == typeof(Transform))
                    continue;
                var customInspectorType = allTypes.FirstOrDefault((t) => t.BaseType.GetGenericTypeDefinition() == typeof(DebugComponentInspectorOfType<>) && t.BaseType.GenericTypeArguments[0] == type);
                if (customInspectorType == null)
                {
                    var container = new VisualElement();
                    var fields = type.GetFields();
                    for (int f = 0; f < fields.Length; f++)
                    {
                        //This part needs to be recursive so it can go trough the classes and serialize recurisevly same goes for arrays/lists
                        var field = fields[f];
                        if (!((!field.IsStatic && field.CustomAttributes.Any((a) => a.AttributeType == typeof(NonSerializedAttribute))) && (field.IsPublic || field.CustomAttributes.Any((a) => a.AttributeType == typeof(SerializeField)))))
                            continue;
                        var fieldInspectorType = allTypes.FirstOrDefault((t) => t.BaseType.GetGenericTypeDefinition() == typeof(DebugFieldInspectorOfType<>) && t.BaseType.GenericTypeArguments[0] == type);
                        if (fieldInspectorType == null)
                        {
                            if (field.FieldType.Name.Contains("Vector"))
                            {
                                fieldInspectorType = typeof(VectorInspector<>).MakeGenericType(field.FieldType);
                            }
                            else if (field.FieldType.IsNumericType())
                            {
                                fieldInspectorType = typeof(NumberInspector<>).MakeGenericType(field.FieldType);
                            }
                            else if (field.FieldType != typeof(string) && field.FieldType != typeof(Transform) && field.FieldType.GetInterface(nameof(IEnumerable)) != null)
                            {
                                Debug.Log("Arrays are not supported yet because you need to make this method recursive to make that happen!");
                            }
                        }
                        var fieldInspector = (DebugInspectorOfTypeBase)System.Activator.CreateInstance(fieldInspectorType, c, field);
                        inspectors.Add(fieldInspector);
                        container.Add(fieldInspector.CreateGUI());
                    }
                    root.Add(container);
                }
                var componentInspector = (DebugInspectorOfTypeBase)System.Activator.CreateInstance(customInspectorType, c);
                inspectors.Add(componentInspector);
                root.Add(componentInspector.CreateGUI());
            }
        }
    }
}
