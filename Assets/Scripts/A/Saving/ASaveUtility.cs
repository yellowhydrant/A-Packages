using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

#pragma warning disable CS0162

namespace A.Saving 
{
    public static class ASaveUtility
    {
        public const bool LogFailure = true;
        public const bool LogSucces = false;
        public const bool Obfuscate = true;

        public static void SaveData<T>(T data, int slot, string fileSaveDirectory, string fileName)
        {
            if (AJsonHelper.IsCollectionType(typeof(T)))
            {
                if (LogFailure)
                    Debug.LogError($"You can't save collections using this method use the overload of {nameof(SaveData)} instead!");
                return;
            }
            var json = JsonUtility.ToJson(data);
            SaveData(json, slot, fileSaveDirectory, fileName);
        }

        public static void SaveData<T>(T[] data, int slot, string fileSaveDirectory, string fileName)
        {
            if (!AJsonHelper.IsCollectionType(typeof(T)))
            {
                if (LogFailure)
                    Debug.LogError($"You can't save non-collections using this method use the overload of {nameof(SaveData)} instead!");
                return;
            }
            var json = AJsonHelper.ToJson(data);
            SaveData(json, slot, fileSaveDirectory, fileName);
        }

        public static void SaveData(string json, int slot, string fileSaveDirectory, string fileName)
        {
            if (json == null || json == "{}")
            {
                if (LogFailure)
                    Debug.LogError($"Save error: this type can't be serialized!");
                return;
            }
            var path = GetPath(slot, fileSaveDirectory, fileName);
            var file = File.Create(path);
            var binary = new BinaryFormatter();
            binary.Serialize(file, Encrypt(json));
            file.Close();
            if (LogSucces)
                Debug.Log($"Save file was made succesfully at path: {path}!");
        }

        public static T LoadData<T>(T data, int slot, string fileSaveDirectory, string fileName)
        {
            if (AJsonHelper.IsCollectionType(typeof(T)))
            {
                if (LogFailure)
                    Debug.LogError($"You can't save collections using this method use the overload of {nameof(LoadData)} instead!");
                return default(T);
            }
            var json = LoadData(slot, fileSaveDirectory, fileName);
            return JsonUtility.FromJson<T>(json);
        }
        public static T[] LoadData<T>(T[] data, int slot, string fileSaveDirectory, string fileName)
        {
            if (!AJsonHelper.IsCollectionType(typeof(T)))
            {
                if (LogFailure)
                    Debug.LogError($"You can't save collections using this method use the overload of {nameof(LoadData)} instead!");
                return default(T[]);
            }
            var json = LoadData(slot, fileSaveDirectory, fileName);
            return AJsonHelper.FromJson<T>(json);
        }

        public static void LoadDataOverwrite<T>(T data, int slot, string fileSaveDirectory, string fileName)
        {
            if (AJsonHelper.IsCollectionType(typeof(T)))
            {
                if (LogFailure)
                    Debug.LogError($"You can't save collections using this method use the overload of {nameof(LoadData)} instead!");
                return;
            }
            var json = LoadData(slot, fileSaveDirectory, fileName);
            JsonUtility.FromJsonOverwrite(json, data);
        }

        public static void DeleteData(int slot)
        {
            var path = GetDirectory(slot, null);
            Directory.Delete(path, true);
        }

        public static string LoadData(int slot, string fileSaveDirectory, string fileName)
        {
            var path = GetPath(slot, fileSaveDirectory, fileName);
            if (File.Exists(path))
            {
                var file = File.Open(path, FileMode.Open);
                var binary = new BinaryFormatter();
                var json = (string)binary.Deserialize(file);
                file.Close();
                return Decrypt(json);
            }
            else 
            {
                if (LogSucces)
                    Debug.Log($"Loading error: file not found at {path}");
                return null;
            }
        }

        static string Encrypt(string msg)
        {
            if (Obfuscate)
                return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(msg));
            else
                return msg;
        }
        static string Decrypt(string msg)
        {
            if (Obfuscate)
                return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(msg));
            else
                return msg;
        }

        static string GetDirectory(int slot, string fileSaveDirectory)
        {
            var path = Path.Combine(Application.persistentDataPath, ASavingConstants.SaveSlotsFolderName, string.Format(ASavingConstants.SaveSlotFolderName, slot.ToString()));
            if (!string.IsNullOrEmpty(fileSaveDirectory))
                path = Path.Combine(path, fileSaveDirectory);
            Directory.CreateDirectory(path);
            return path;

        }

        static string GetPath(int slot, string fileSaveDirectory, string fileName)
        {
            return Path.Combine(GetDirectory(slot, fileSaveDirectory), fileName + ASavingConstants.SaveFileExtension);
        }
    }
}
