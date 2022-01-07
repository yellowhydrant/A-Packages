using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ASavingConstants
{
    public const string SaveSlotsFolderName = "Saves";
    public const string SaveSlotFolderName = "SaveSlot{0}";
    public const string SaveFileExtension = ".savedata";
    public static readonly string[] FoldersContainingSavableObjects = new string[] { "Assets/ScriptableObjects" };
}
