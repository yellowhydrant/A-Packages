#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace A
{
    public static class AEditorUtills
    {
        public static string GetPathToThisFile<T>(string scriptName = nameof(T)) where T : ScriptableObject
        {
            var dummy = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy));
            Object.DestroyImmediate(dummy);
            return path.Substring(0, path.LastIndexOf($"/{scriptName}.cs"));
        }

        public static string GetPathToThisFile(System.Type type, string scriptName)
        {
            if (!type.IsSubclassOf(typeof(ScriptableObject)))
            {
                Debug.LogError("This type does not inherit from ScriptableObject!");
                return null;
            }
            var dummy = ScriptableObject.CreateInstance(type);
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(dummy));
            Object.DestroyImmediate(dummy);
            return path.Substring(0, path.LastIndexOf($"/{scriptName}.cs"));
        }
    }
}
#endif
