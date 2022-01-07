using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor;
#endif

namespace A.Saving
{
    //NOTE: The savable values assume that the values don't have any collections in them
    //NOTE: Maybe it would be handy to add array and list versions of the current basic values
    public sealed class ASaveManager : ScriptableObject
    {
        [SerializeField] ASavableObject[] cachedValues;
        static ASavableObject[] Values = System.Array.Empty<ASavableObject>();

        [SerializeField][Range(0, MaxSaveSlotAmount)] int slot;

        const int MaxSaveSlotAmount = 10;

#if UNITY_EDITOR
        static string RootDirectory => AEditorUtills.GetPathToThisFile<ASaveManager>(nameof(ASaveManager));
#endif

        const string ResourcesName = "SaveManager";

#if UNITY_EDITOR
        class ASaveManagerBuildLoad : IPreprocessBuildWithReport
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
            var manager = AssetDatabase.LoadAssetAtPath<ASaveManager>(path);
            if (manager == null)
            {
                manager = ScriptableObject.CreateInstance<ASaveManager>();
                AssetDatabase.CreateAsset(manager, path);
                AssetDatabase.SaveAssets();
            }
            //Add all of the savableobject assets to the cache
            manager.cachedValues = AAssetDatabaseHelper.LoadAllAssetsInFolders<ASavableObject>(ASavingConstants.FoldersContainingSavableObjects);
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            var manager = Resources.Load<ASaveManager>(ResourcesName);
            if (manager == null || manager.cachedValues == null)
            {
                Debug.LogError("Faulty execution order");
                return;
            }
            Values = manager.cachedValues;
        }

        [MyBox.ButtonMethod]
        void Save() => SaveAllObjects(slot);
        public static void SaveAllObjects(int slot = 0)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                ASaveUtility.SaveData(Values[i].ToJson(), slot, Values[i].SaveSlotSubDirectory, Values[i].guid);
            }
        }

        [MyBox.ButtonMethod]
        void Load() => LoadAllObjects(slot);
        public static void LoadAllObjects(int slot = 0)
        {
            for (int i = 0; i < Values.Length; i++)
            {
                Values[i].FromJson(ASaveUtility.LoadData(slot, Values[i].SaveSlotSubDirectory, Values[i].guid));
            }
        }

        [MyBox.ButtonMethod]
        void Reset() => ResetAllObjects();
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
//const string SaveSlotName = "Slot{0}";

//class Wrapper
//{
//    public Data[] data;
//}

//class Data
//{
//    public string json;
//    public string guid;
//}

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

