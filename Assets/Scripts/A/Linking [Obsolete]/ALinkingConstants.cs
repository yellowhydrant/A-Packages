using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.Linking
{
    public sealed class ALinkingConstants : ScriptableObject
    {
        public const string ComponentMenuRoot = "Linking";

#if UNITY_EDITOR
        public static string StyleSheetPath = RootDirectory + "/" + StyleSheetLocalPath;
        public static string VisualTreeAssetPath = RootDirectory + "/" + VisualTreeAssetLocalPath;

        const string StyleSheetLocalPath = "Editor/LinkEditor.uss";
        const string VisualTreeAssetLocalPath = "Editor/LinkEditor.uxml";

        static string RootDirectory => A.Editor.AEditorUtility.GetPathToThisFile<ALinkingConstants>(nameof(ALinkingConstants));
#endif
    }
}
