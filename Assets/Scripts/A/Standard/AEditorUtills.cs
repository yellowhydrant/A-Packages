using UnityEditor;
using UnityEngine;

namespace A
{
    public static class AEditorUtills
    {
        public static string GetPathToThisFile<T>(string scriptName) where T : ScriptableObject
        {
            var dummy = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy));
            Object.DestroyImmediate(dummy);
            return path.Substring(0, path.LastIndexOf($"/{scriptName}.cs"));
        }
    }
}
