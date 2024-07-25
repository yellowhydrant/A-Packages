using UnityEngine;

namespace A.Dialogue
{
    public class ARootNodeData : ANodeData
    {
        public override System.Type portType => typeof(Texture2D);

        public override void OnNodeEnter(ADialogueParser parser)
        {
            //
        }

        public override void OnNodeExit(ADialogueParser parser)
        {
            //
        }

        public override void OnNodeUpdate(ADialogueParser parser)
        {
            //
        }
    }
}
