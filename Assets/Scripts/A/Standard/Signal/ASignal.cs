using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/Signal")]
    public class ASignal : ScriptableObject
    {
        List<ASignalListener> listeners = new List<ASignalListener>();

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnSignalRaised();
            }
        }

        public void RegisterListener(ASignalListener listener)
        {
            listeners.Add(listener);
        }

        public void DeregisterListener(ASignalListener listener)
        {
            listeners.Remove(listener);
        }
    }
}