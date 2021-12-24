using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace A.Dialogue
{
    //Add support for multiple registers and figure out how to pass register to nodeData
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ADialogueConstants.AssetMenuRoot + "/" + "Speaker Register", fileName = "New SpeakerRegister")]
    public class ADialogueSpeakerRegister : ScriptableObject
    {
        public ReadOnlyCollection<ADialogueSpeaker> speakers => speakers_.AsReadOnly();
        [SerializeField] List<ADialogueSpeaker> speakers_ = new List<ADialogueSpeaker>();

        public void AddSpeaker()
        {
#if UNITY_EDITOR
            var speaker = ScriptableObject.CreateInstance<ADialogueSpeaker>();
            AssetDatabase.AddObjectToAsset(speaker, this);
            AssetDatabase.SaveAssets();
            speakers_.Add(speaker);
#endif
        }

        public void RemoveSpeaker(ADialogueSpeaker speaker)
        {
#if UNITY_EDITOR
            AssetDatabase.RemoveObjectFromAsset(speaker);
            AssetDatabase.SaveAssets();
            speakers_.Remove(speaker);
#endif
        }
    }
}
