using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Dialogue
{
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ADialogueConstants.AssetMenuRoot + "/" + "Dialogue Graph", fileName = "New DialogueGraph")]
    public partial class ADialogueGraph : ScriptableObject
    {
        public List<ANodeData> nodeData = new List<ANodeData>();
        public List<LinkData> nodeLinks = new List<LinkData>();
        public List<AExposedProperty> exposedProperties = new List<AExposedProperty>();
        public ADialogueSpeakerRegister register;

        public const string Continue = "Continue";

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
