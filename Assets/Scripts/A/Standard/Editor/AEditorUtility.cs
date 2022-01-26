#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace A.Editor
{
    public static class AEditorUtility
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

        [MenuItem("Assets/Select dependants", true, 20)]
        static bool SelectSelectionDependantsValidation()
        {
            return Selection.activeObject != null;
        }

        [MenuItem("Assets/Select dependants", false, 20)]
        static void SelectSelectionDependants()
        {
            Selection.objects = GetDependants(Selection.activeObject);
        }

        [MenuItem("Tools/Test")]
        static void Test()
        {
        }

        public static Object[] GetDependants(Object target)
        {
            //Setup
            var matches = new List<Object>();
            //Get all objects project wide
            Object[] allObjects = Resources.FindObjectsOfTypeAll<Object>();
            //Loop trough every object and check if it depends on the target
            for (int o = 0; o < allObjects.Length; o++)
            {
                Object[] dependencies = EditorUtility.CollectDependencies(new Object[] { allObjects[o] });
                for (int d = 0; d < dependencies.Length; d++)
                    if (dependencies[d] == target)
                        matches.Add(allObjects[o]);
            }
            return matches.ToArray();
        }
    }
}
#endif
