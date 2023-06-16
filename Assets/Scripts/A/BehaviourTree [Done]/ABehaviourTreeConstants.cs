using System.Collections;
using System.Collections.Generic;
using A.Editor;
using UnityEngine;

namespace A.BehaviourTree
{
    public class ABehaviourTreeConstants : ScriptableObject
    {
        public const string ComponentMenuRoot = "Behaviour Tree";
        public const string AssetMenuRoot = "Behaviour Tree";

#if UNITY_EDITOR
        public static string TreeStyleSheetPath = RootDirectory + "/" + TreeStyleSheetLocalPath;
        public static string TreeVisualTreeAssetPath = RootDirectory + "/" + TreeVisualTreeAssetLocalPath;

        const string TreeStyleSheetLocalPath = "Editor/UI/BehaviourTreeEditorStyle.uss";
        const string TreeVisualTreeAssetLocalPath = "Editor/UI/BehaviourTreeEditor.uxml";

        public static string NodeStyleSheetPath = RootDirectory + "/" + NodeStyleSheetLocalPath;
        public static string NodeVisualTreeAssetPath = RootDirectory + "/" + NodeVisualTreeAssetLocalPath;

        const string NodeStyleSheetLocalPath = "Editor/UI/NodeViewStyle.uss";
        const string NodeVisualTreeAssetLocalPath = "Editor/UI/NodeView.uxml";

        static string RootDirectory => AEditorUtility.GetPathToThisFile<ABehaviourTreeConstants>(nameof(ABehaviourTreeConstants));
#endif
    }
}