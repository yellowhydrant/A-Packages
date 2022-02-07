using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using A.Extensions;

#pragma warning disable CS0162

namespace A.Saving 
{
    public static class ASaveUtility
    {
        public const bool LogSucces = false;
        public const bool Obfuscate = true;
        public const bool PrettyPrint = false;
        //public const System.Text.Encoding Encoding = default;

        public static void SaveData<T>(T data, int slot, string dataSlotSubDirectory, string fileName)
        {
            if (typeof(T).IsCollectionType())
                throw new System.ArgumentException($"You can't save collections using this method use {nameof(SaveEnumerable)} instead!", nameof(data));

            var json = JsonUtility.ToJson(data);
            SaveJson(json, slot, dataSlotSubDirectory, fileName);
        }

        public static void SaveEnumerable<T>(IEnumerable<T> data, int slot, string dataSlotSubDirectory, string fileName)
        {
            if (typeof(T) == typeof(char) && data as string != null)
                throw new System.ArgumentException($"{typeof(string).Name} isn't a valid {nameof(IEnumerable<T>)} argument!");

            var json = AJsonHelper.ToJson(data, PrettyPrint);
            SaveJson(json, slot, dataSlotSubDirectory, fileName);
        }

        public static void SaveJson(string json, int slot, string dataSlotSubDirectory, string fileName)
        {
            if (json == null || json == "{}")
                throw new System.ArgumentNullException($"Save Error: This type isn't supported by {nameof(JsonUtility)}! " +
                    $"Try serializing this type using a custom wrapper by directly calling {nameof(SaveJson)}.", nameof(json));

            var path = GetFilePath(slot, dataSlotSubDirectory, fileName);
            try
            {
                File.WriteAllText(path, json);
            }
            catch(System.Exception e)
            {
                throw new System.Exception($"Save Error: an error occured while saving file at {path}!", e);
            }
            if (LogSucces)
                Debug.Log($"Save file was made succesfully at path: {path}!");
        }

        public static T LoadData<T>(int slot, string fileSaveDirectory, string fileName)
        {
            if (typeof(T).IsCollectionType())
                throw new System.ArgumentException($"Load Error: You can't load collections using this method, use {nameof(LoadEnumerable)} instead!", "Data Type");

            var json = LoadJson(slot, fileSaveDirectory, fileName);
            return JsonUtility.FromJson<T>(json);
        }

        public static IEnumerable<T> LoadEnumerable<T>(int slot, string dataSlotSubDirectory, string fileName)
        {
            var json = LoadJson(slot, dataSlotSubDirectory, fileName);
            return AJsonHelper.FromJson<T>(json);
        }

        public static void LoadDataOverwrite<T>(T data, int slot, string dataSlotSubDirectory, string fileName)
        {
            if (typeof(T).IsCollectionType())
                throw new System.ArgumentException($"Load Error: You can't load collections using this method, use {nameof(LoadEnumerable)} instead!", "Data Type");

            var json = LoadJson(slot, dataSlotSubDirectory, fileName);
            JsonUtility.FromJsonOverwrite(json, data);
        }

        public static string LoadJson(int slot, string dataSlotSubDirectory, string fileName)
        {
            var path = GetFilePath(slot, dataSlotSubDirectory, fileName);
            try
            {
                var json = File.ReadAllText(path);
                if (LogSucces)
                    Debug.Log($"Save file was made succesfully at path: {path}!");
                return json;
            }
            catch(System.Exception e)
            {
                throw new System.Exception($"Load error: An error occured while loading file at {path}", e);
            }
        }

        public static void DeleteDataSlot(int slot)
        {
            var path = GetDataSlotDirectory(slot, null);
            Directory.Delete(path, true);
        }

        static string GetDataSlotDirectory(int slot, string dataSlotSubDirectory)
        {
            var path = Path.Combine(Application.persistentDataPath, ASavingConstants.DataSlotsFolderName, string.Format(ASavingConstants.DataSlotFolderNameFormat, slot));
            if (!string.IsNullOrEmpty(dataSlotSubDirectory))
                path = Path.Combine(path, dataSlotSubDirectory);
            Directory.CreateDirectory(path);
            return path;
        }

        static string GetFilePath(int slot, string dataSlotSubDirectory, string fileName)
        {
            return Path.Combine(GetDataSlotDirectory(slot, dataSlotSubDirectory), fileName + ASavingConstants.DataFileExtension);
        }

        //static string Encrypt(string msg)
        //{
        //    if (Obfuscate)
        //        return System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(msg));
        //    else
        //        return msg;
        //}
        //static string Decrypt(string msg)
        //{
        //    if (Obfuscate)
        //        return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(msg));
        //    else
        //        return msg;
        //}
    }
}
