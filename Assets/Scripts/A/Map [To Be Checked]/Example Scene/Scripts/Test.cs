using System.Collections;
using System.Collections.Generic;
using A.Map;
using UnityEngine;

public class Test : MonoBehaviour
{
    public APin pin;

    [ContextMenu("Add Pin To Map")]
    IEnumerator Start()
    {
        yield return null;
        AMap.instance.AddPin(pin);
    }

}

