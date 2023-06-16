using UnityEngine;
using System.Collections.Generic;
using A.Extensions;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
using A.Editor;
#endif

namespace A.Inventory
{
    public sealed class AItemCache : ScriptableObject
    {
        [SerializeField, HideInInspector] AItem[] cachedItems;
        static Dictionary<string, AItem> items;

        static AItem missingItem = null;
        static AItem emptyItem = null;

        const string ResourcesName = "ItemCache";

#if UNITY_EDITOR
        static string RootDirectory => AEditorUtility.GetPathToThisFile<AItemCache>(nameof(AItemCache));

        class AItemCacheBuildLoad : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                CacheValues();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void CacheValues()
        {
            //Load the asset onbuild and on scene start
            var path = System.IO.Path.Combine(RootDirectory, nameof(Resources));
            System.IO.Directory.CreateDirectory(path);
            path = System.IO.Path.Combine(path, $"{ResourcesName}.asset");
            var cache = AssetDatabase.LoadAssetAtPath<AItemCache>(path);
            if (cache == null)
            {
                cache = ScriptableObject.CreateInstance<AItemCache>();
                AssetDatabase.CreateAsset(cache, path);
                AssetDatabase.SaveAssets();
            }
            //Add all of the savableobject assets to the cache
            cache.cachedItems = AAssetDatabaseHelper.LoadAllAssetsInFolders<AItem>(AInventoryConstants.FoldersContainingSavableObjects);

            Init();
        }
#endif
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static void Init()
        {
            var cache = Resources.Load<AItemCache>(ResourcesName);
            items = new Dictionary<string, AItem>();
            for (int i = 0; i < cache.cachedItems.Length; i++)
            {
                items.Add(cache.cachedItems[i].guid, cache.cachedItems[i]);
            }
        }

        public static AItem GetItem(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return emptyItem;

            if (guid.IsValidGuid())
            {
                return items[guid];
            }
            else
            {
                Debug.LogError("Error: Invalid guid, item not found!");
                return missingItem;
            }
        }
    }
}

