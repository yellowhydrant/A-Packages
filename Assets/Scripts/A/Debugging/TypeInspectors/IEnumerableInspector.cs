using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class IEnumerableInspector<T> : DebugFieldInspectorOfType<T> where T : IEnumerable
    {
        public IEnumerableInspector(object target, FieldInfo info) : base(target, info)
        {

        }

        public override VisualElement CreateGUI()
        {
            var listview = new ListView();
            var enumerable = target as IEnumerable;
            foreach (var item in enumerable)
            {
                

            }

            return null;
        }

        public override void UpdateGUI()
        {
        }
    }

    public class IEnumerableInspector : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<IEnumerableInspector, UxmlTraits>
        {
            public UxmlFactory()
            {

            }
        }

        public IEnumerableInspector()
        {
            var list = new List<int>() { 1, 2, 3, 4, 5, 6, 7};
            var listview = new ListView();
            listview.makeItem = () =>
            {
                var ve = new VisualElement();
                ve.Add(new Label());
                return ve;
            };
            listview.bindItem = (elem, index) =>
            {
                (elem.ElementAt(0) as Label).text = index.ToString();
            };
            listview.itemsSource = list;
            Add(listview);
            schedule.Execute(() => listview.Refresh());
        }
    }
}
