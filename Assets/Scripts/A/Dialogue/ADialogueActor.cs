using UnityEngine;

namespace A.Dialogue
{
    public class ADialogueActor : ScriptableObject
    {
        public string guid = System.Guid.NewGuid().ToString();
        public Sprite sprite;

        public static ADialogueActor NullSpeaker 
        {
            get 
            {
                if (nullSpeaker == null)
                {
                    nullSpeaker = ScriptableObject.CreateInstance<ADialogueActor>();
                    nullSpeaker.guid = null;
                    nullSpeaker.name = "Unassigned";
                    nullSpeaker.sprite = null; //TODO: Maybe add default sprite
                }
                return nullSpeaker;
            }
        }
        static ADialogueActor nullSpeaker;
    }
}
