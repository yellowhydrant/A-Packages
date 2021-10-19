using System.Collections.Generic;
using UnityEngine;

namespace A.Linking
{
    public interface ILinkable
    {
        public Link[] links { get; set; }
        public UnityEngine.Vector2 groupPosition { get; set; }

        [System.Serializable]
        public class Link
        {
            public string name = " ";
            public List<string> inputGUIDs;
            public string[] outputGUIDs;
            public List<string> oldOutputGUIDs;
            [HideInInspector]
            public Vector2 nodePosition;
        }
    }
}
