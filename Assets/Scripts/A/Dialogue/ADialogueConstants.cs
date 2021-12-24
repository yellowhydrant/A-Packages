using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Dialogue
{
    public sealed class ADialogueConstants : ScriptableObject
    {

        public const string ComponentMenuRoot = "Dialogue";
        public const string AssetMenuRoot = "Dialogue";

#if UNITY_EDITOR
        public static string StyleSheetPath = RootDirectory + "/" + StyleSheetLocalPath;
        public static string VisualTreeAssetPath = RootDirectory + "/" + VisualTreeAssetLocalPath;

        const string StyleSheetLocalPath = "Editor/DialogueGraphEditorStyle.uss";
        const string VisualTreeAssetLocalPath = "Editor/DialogueGraphEditor.uxml";

        static string RootDirectory => AEditorUtills.GetPathToThisFile<ADialogueConstants>(nameof(ADialogueConstants));
#endif
    }
}
