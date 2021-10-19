using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Dialogue
{
    [CreateAssetMenu(menuName = "DialogueGraph", fileName = "DialogueGraph")]
    public partial class ADialogueGraph : ScriptableObject
    {
        public List<NodeData> nodeData = new List<NodeData>();
        public List<LinkData> nodeLinks = new List<LinkData>();
        public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();

        [System.Serializable]
        public class LinkData
        {
            public string baseGUID;
            public string portName;
            public string targetGUID;

            public LinkData(string baseGuid, string port, string targetGuid)
            {
                baseGUID = baseGuid;
                portName = port;
                targetGUID = targetGuid;
            }
        }
    }
}
