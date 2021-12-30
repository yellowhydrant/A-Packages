using System.Collections;
using System.Collections.Generic;
using A.Map;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] APin pin;

    [ContextMenu("Add Pin To Map")]
    void AddPinToMap()
    {
        AMap.instance.AddPin(pin);
    }
}
