using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ASavingConstants
{
    public const string DataSlotsFolderName = "Saves";
    public const string DataSlotFolderNameFormat = "SaveSlot{0}";
    public const string DataFileExtension = ".savedata";
    public static readonly string[] FoldersContainingSavableObjects = new string[] { "Assets" };//"Assets/ScriptableObjects" };
}
