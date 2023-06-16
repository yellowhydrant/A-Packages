#undef SINGLE_FILE_SAVESLOTS //TODO: Make it so if defined the files will be saved in saves instead of saves/saveslots
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using A.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
#endif

#pragma warning disable CS0162

namespace A.Saving
{
    public sealed class ASaveManager : ScriptableObject
    {
        [SerializeField] ASavableObject[] cachedValues;
        static ASavableObject[] Values = System.Array.Empty<ASavableObject>();
        static Dictionary<string, ASavableObject> ValueLookupDictionary;

        [SerializeField][Range(0, MaxDataSlotAmount)] int slot;

        const int MaxDataSlotAmount = 10;

        const string ResourcesFileName = "SaveManager";

#if SINGLE_FILE_SAVESLOTS
        const string DataSlotName = "Save{0}";

        class Data
        {
            public string json;
            public string guid;
        }
#endif

#if UNITY_EDITOR
        static string RootDirectory => AEditorUtility.GetPathToThisFile<ASaveManager>(nameof(ASaveManager));

        const bool OnlySaveUsedObjects = false;

        static ASaveManager GetRuntimeManager()
        {
            var path = System.IO.Path.Combine(RootDirectory, nameof(Resources));
            System.IO.Directory.CreateDirectory(path);
            path = System.IO.Path.Combine(path, $"{ResourcesFileName}.asset");
            var manager = AssetDatabase.LoadAssetAtPath<ASaveManager>(path);
            if (manager == null)
            {
                manager = ScriptableObject.CreateInstance<ASaveManager>();
                AssetDatabase.CreateAsset(manager, path);
                AssetDatabase.SaveAssets();
            }
            return manager;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void CacheValues()
        {
            //Load the asset onbuild and on scene start
            var manager = GetRuntimeManager();
            //Add all of the savableobject assets to the cache
            manager.cachedValues = AAssetDatabaseHelper.LoadAllAssetsInFolders<ASavableObject>(ASavingConstants.FoldersContainingSavableObjects);
            if (OnlySaveUsedObjects)
                manager.cachedValues = manager.cachedValues.Where((obj) => AEditorUtility.GetDependants(obj).Length > 0).ToArray();

            Init();
        }

        class ASaveManagerBuildLoad : IPreprocessBuildWithReport
        {
            public int callbackOrder => 0;

            public void OnPreprocessBuild(BuildReport report)
            {
                CacheValues();
            }
        }
#endif
#if !UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        static void Init()
        {
            var manager = Resources.Load<ASaveManager>(ResourcesFileName);
            //Object.DontDestroyOnLoad(manager);//Need to test if this is neccesary
            Values = manager.cachedValues;
            ValueLookupDictionary = Values.ToDictionary((v) => v.guid);
        }

        [MyBox.ButtonMethod]
        void Save()
        {
            Init();
            SaveAllObjects(slot);
        }

        public static void SaveAllObjects(int slot = 0)
        {
#if SINGLE_FILE_SAVESLOTS
            var data = new Data[Values.Length];
            for (int i = 0; i < Values.Length; i++)
                data[i] = new Data { json = Values[i].ToJson(), guid = Values[i].guid };

            ASaveUtility.SaveEnumerable(data, slot, null, string.Format(DataSlotName, slot));
#else
            for (int i = 0; i < Values.Length; i++)
            {
                ASaveUtility.SaveJson(Values[i].ToJson(), slot, Values[i].DataSlotSubDirectory, Values[i].guid);
            }
#endif
        }

        [MyBox.ButtonMethod]
        void Load()
        {
            Init();
            LoadAllObjects(slot);
        }
        public static void LoadAllObjects(int slot = 0)
        {
#if SINGLE_FILE_SAVESLOTS
            var data = ASaveUtility.LoadEnumerable<Data>(slot, null, string.Format(DataSlotName, slot));
            foreach (var item in data)
            {
                if(ValueLookupDictionary.TryGetValue(item.guid, out var value))
                    value.FromJson(item.json);
            }
#else
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i].FromJson(ASaveUtility.LoadJson(slot, Values[i].DataSlotSubDirectory, Values[i].guid));
            }
#endif
        }

        [MyBox.ButtonMethod]
        void Delete() => DeleteDataSlot(slot);
        public static void DeleteDataSlot(int slot)
        {
            ASaveUtility.DeleteDataSlot(slot);
        }

        [MyBox.ButtonMethod]
        void Reset()
        {
            Init();
            ResetAllObjects();
        }
        public static void ResetAllObjects()
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i].ResetValue();
            }
        }
    }
}

