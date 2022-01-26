//#define SINGLE_FILE_SAVESLOTS //TODO: Make it so if defined the files will be saved in saves instead of saves/saveslots
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

        [SerializeField][Range(0, MaxSaveSlotAmount)] int slot;

        const int MaxSaveSlotAmount = 10;

        const string ResourcesName = "SaveManager";

#if SINGLE_FILE_SAVESLOTS
        const string SaveSlotName = "Save{0}";

        class Wrapper
        {
            public Data[] data;
        }

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
            path = System.IO.Path.Combine(path, $"{ResourcesName}.asset");
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
            var manager = Resources.Load<ASaveManager>(ResourcesName);
            Values = manager.cachedValues;
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
            var wrapper = new Wrapper();
            wrapper.data = new Data[Values.Length];
            for (int i = 0; i < Values.Length; i++)
            {
                wrapper.data[i] = new Data { json = Values[i].ToJson(), guid = Values[i].guid };
            }
            ASaveUtility.SaveData(JsonUtility.ToJson(wrapper), slot, null, string.Format(SaveSlotName, slot));
#else
            for (int i = 0; i < Values.Length; i++)
            {
                ASaveUtility.SaveData(Values[i].ToJson(), slot, Values[i].SaveSlotSubDirectory, Values[i].guid);
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
            var wrapper = JsonUtility.FromJson<Wrapper>(ASaveUtility.LoadData(slot, null, string.Format(SaveSlotName, slot)));
            for (int i = 0; i < wrapper.data.Length; i++)
            {
                var val = Values.FirstOrDefault((v) => v.guid == wrapper.data[i].guid);
                if(val != null)
                    val.FromJson(wrapper.data[i].json);
            }
#else
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i].FromJson(ASaveUtility.LoadData(slot, Values[i].SaveSlotSubDirectory, Values[i].guid));
            }
#endif
        }

        [MyBox.ButtonMethod]
        void Delete() => DeleteSaveSlot(slot);
        public static void DeleteSaveSlot(int slot)
        {
            ASaveUtility.DeleteData(slot);
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

//Implementation of single file saving

//void Save(int slot = 0)
//{
//    var wrapper = new Wrapper();
//    wrapper.data = new Data[values.Length];
//    for (int i = 0; i < values.Length; i++)
//    {
//        wrapper.data[i] = new Data { json = values[i].ToJson(), guid = values[i].guid };
//    }
//    ASaveUtility.SaveData(wrapper, slot, "", string.Format(SaveSlotName, slot));
//}

//void Load(int slot = 0)
//{
//    var wrapper = JsonUtility.FromJson<Wrapper>(ASaveUtility.LoadData(slot, "", string.Format(SaveSlotName, slot)));
//    for (int i = 0; i < wrapper.data.Length; i++)
//    {
//        var val = values.First((v) => v.guid == wrapper.data[i].guid);
//        val.FromJson(wrapper.data[i].json);
//    }
//}

