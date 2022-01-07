using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
#endif

namespace A.Inventory
{
    [CreateAssetMenu]
    public class AItemCache : ScriptableObject
    {
        [SerializeField] AItem[] cachedItems;
        public static Dictionary<string, AItem> Items { get; private set; }

        const string ResourcesName = "ItemCache";

#if UNITY_EDITOR
        static string RootDirectory => AEditorUtills.GetPathToThisFile<AItemCache>(nameof(AItemCache));

        class AItemCacheBuildLoad : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                CacheValues();
            }
        }
#endif

#if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void CacheValues()
        {
            //Load the asset onbuild and on scene start
            var path = System.IO.Path.Combine(RootDirectory, nameof(Resources), $"{ResourcesName}.asset");
            var manager = AssetDatabase.LoadAssetAtPath<AItemCache>(path);
            if (manager == null)
            {
                manager = ScriptableObject.CreateInstance<AItemCache>();
                AssetDatabase.CreateAsset(manager, path);
                AssetDatabase.SaveAssets();
            }
            //Add all of the savableobject assets to the cache
            manager.cachedItems = AAssetDatabaseHelper.LoadAllAssetsInFolders<AItem>(AInventoryConstants.FoldersContainingSavableObjects);
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            var manager = Resources.Load<AItemCache>(ResourcesName);
            if (manager == null || manager.cachedItems == null)
            {
                Debug.LogError("Faulty execution order");
                return;
            }
            Items = new Dictionary<string, AItem>();
            for (int i = 0; i < manager.cachedItems.Length; i++)
            {
                Items.Add(manager.cachedItems[i].guid, manager.cachedItems[i]);
            }
        }
    }
}

