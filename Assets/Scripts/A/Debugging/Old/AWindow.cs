using UnityEngine;
using UnityEngine.UIElements;

namespace A.Debugging
{
    public class AWindow : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AWindow, UxmlTraits> { }

        public VisualElement controlBar { get; }
        public Label titleText { get; }
        public Button closeButton { get; }
        public Button shrinkExpandButton { get; }
        public bool isExpanded { get; set; } = true;
        public Button refreshButton { get; }
        public VisualElement ccContainer { get; }
        public override VisualElement contentContainer { get; }

        public AWindow()
        {
            Resources.Load<VisualTreeAsset>("AWindowUI").CloneTree(this);
            controlBar = this.Q<VisualElement>("control-bar");
            contentContainer = this.Q<VisualElement>("content-container");
            ccContainer = this.Q<VisualElement>("content-container-container");
            titleText = controlBar.Q<Label>("title-text");
            closeButton = controlBar.Q<Button>("close-button");
            shrinkExpandButton = controlBar.Q<Button>("shrink-expand-button");
            shrinkExpandButton.clicked += () => 
            {
                isExpanded = !isExpanded;
                shrinkExpandButton.text = isExpanded ? "–" : "⛶";
                if (isExpanded)
                    ccContainer.Add(contentContainer);
                else
                    ccContainer.Remove(contentContainer);
                ccContainer.style.flexGrow = isExpanded ? 1 : 0;
            };
            refreshButton = controlBar.Q <Button>("refresh-button");
            this.AddManipulator(new ADraggable(controlBar));
        }
    }
}
