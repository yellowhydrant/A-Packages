using UnityEngine;

namespace A.Dialogue
{
    public class ADialogueSpeaker : ScriptableObject
    {
        public string guid = System.Guid.NewGuid().ToString();
        //public new string name;
        public Sprite sprite;

        public static ADialogueSpeaker NullSpeaker
        {
            get 
            {
                if (nullSpeaker != null)
                {
                    return nullSpeaker;
                }
                else
                {
                    var speaker = ScriptableObject.CreateInstance<ADialogueSpeaker>();
                    speaker.guid = null;
                    speaker.name = "Unassigned";
                    speaker.sprite = null; //TODO: Maybe add default sprite
                    return speaker;
                }
            }
        }
        static ADialogueSpeaker nullSpeaker;
    }
}
