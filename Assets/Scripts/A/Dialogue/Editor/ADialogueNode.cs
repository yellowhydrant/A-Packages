#if UNITY_EDITOR
using UnityEngine.UIElements;

namespace A.Dialogue.Editor
{
    public class ADialogueNode : UnityEditor.Experimental.GraphView.Node
    {
        public string GUID;
        public bool entryPoint;

        public ANodeData nodeData;

        public ADialogueNode()
        {

        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
            evt.menu.AppendAction("Turn into branch node", (ctx) => TurnIntoBranchNode());
        }

        public void TurnIntoBranchNode()
        {
            var imguiContainer = mainContainer.Q<IMGUIContainer>();
            if(imguiContainer != null)
                mainContainer.Remove(imguiContainer);
            var label = mainContainer.Q<Label>();
            label.text = "Branch";
            var addButton = mainContainer.Q<Button>();
            addButton.text = "New Chance";
            nodeData.isBranch = true;
        }
    }
}
#endif
