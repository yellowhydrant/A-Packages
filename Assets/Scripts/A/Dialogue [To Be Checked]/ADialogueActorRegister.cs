using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace A.Dialogue
{
    //Add support for multiple registers and figure out how to pass register to nodeData
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ADialogueConstants.AssetMenuRoot + "/" + "Actor Register", fileName = "New ActorRegister")]
    public class ADialogueActorRegister : ScriptableObject
    {
        public ReadOnlyCollection<ADialogueActor> actors => actors_.AsReadOnly();
        [SerializeField] List<ADialogueActor> actors_ = new List<ADialogueActor>();


        public void AddActor()
        {
            AddActor(out var _);
        }
        public void AddActor(out ADialogueActor actor)
        {
#if UNITY_EDITOR
            actor = ScriptableObject.CreateInstance<ADialogueActor>();
            AssetDatabase.AddObjectToAsset(actor, this);
            AssetDatabase.SaveAssets();
            actors_.Add(actor);
#endif
        }

        public void RemoveActor(ADialogueActor actor)
        {
#if UNITY_EDITOR
            AssetDatabase.RemoveObjectFromAsset(actor);
            AssetDatabase.SaveAssets();
            actors_.Remove(actor);
#endif
        }
    }
}
