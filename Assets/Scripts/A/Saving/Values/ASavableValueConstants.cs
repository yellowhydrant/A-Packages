using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ASavableValueConstants
{
    public const string AssetMenuRoot = "Saving/Values";
    /// <summary>
    /// At the start of the program all assets in this directory will be loaded and cached for saving and loading.
    /// </summary>
    public const string ResourcesLoadDirectory = "SavableObjects";
    /// <summary>
    /// Assets will be saved and loaded to/from this subdirectory of the main save slot folder.
    /// </summary>
    public const string SaveSlotSubDirectory = "Values";
}
