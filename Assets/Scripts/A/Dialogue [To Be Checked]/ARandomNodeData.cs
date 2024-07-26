using UnityEngine;

namespace A.Dialogue
{
    public class ARandomNodeData : ANodeData
    {
        public override bool floatChoiceNames => true;
        public override System.Type portType => typeof(string);
        public float portWeightSum => GetPortWeightSum();

        public override void OnNodeEnter(ADialogueParser parser)
        {
            //TODO: make it weighted using the portnames from choices[]
            //parser.ContinueDialogue(choices[Random.Range(0, choices.Length)]);
            var sum = portWeightSum;
            var num = Random.value;
            var prev = -0.00001f;
            for (int i = 0; i < choices.Length; i++) 
            {
                var cur = float.Parse(choices[i].portName) / sum + prev;
                if (num > prev && num <= cur)
                {
                    parser.ContinueDialogue(choices[i]);
                    return;
                }
                prev = cur;
            }
        }

        public override void OnNodeExit(ADialogueParser parser)
        {
            //
        }

        public override void OnNodeUpdate(ADialogueParser parser)
        {
            //
        }

        public float GetPortWeightSum()
        {
            var sum = 0f;
            for (int i = 0; i < choices.Length; i++)
                sum += float.Parse(choices[i].portName);
            return sum;
        }
    }
}
