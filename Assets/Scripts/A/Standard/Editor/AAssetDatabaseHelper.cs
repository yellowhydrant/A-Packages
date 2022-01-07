using System.Collections;
using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AAssetDatabaseHelper
{
    public static T[] LoadAllAssetsInFolders<T> (params string[] folders) where T : Object
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, folders);
        var assets = new T[guids.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
        }
        return assets;
    }
}
#endif
