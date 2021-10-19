using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GUID")]
public class AGUID : ScriptableObject
{
    [HideInInspector] public string GUID;

    private void Awake()
    {
        if (GUID == null)
            GUID = System.Guid.NewGuid().ToString();
    }

    private void OnValidate()
    {
        Awake();
    }
}
