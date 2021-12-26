using System;
using UnityEditor;
using UnityEngine;

namespace A.Editor
{
    public class AAssetModificationProcessorCallbacks : UnityEditor.AssetModificationProcessor
    {
        public static Func<string, RemoveAssetOptions, AssetDeleteResult> OnWillDelete;
        public static Func<string, string, AssetMoveResult> OnWillMove;
        public static Func<string[], string[]> OnWillSave;

        static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions options)
        {
            return OnWillDelete == null ? AssetDeleteResult.DidNotDelete : OnWillDelete.Invoke(path, options);
        }

        static AssetMoveResult OnWillMoveAsset(string sourcePath, string destPath)
        {
            return OnWillMove == null ? AssetMoveResult.DidNotMove : OnWillMove.Invoke(sourcePath, destPath);
        }

        static string[] OnWillSaveAssets(string[] paths)
        {
            return OnWillSave == null ? paths : OnWillSave.Invoke(paths);
        }
    }
}