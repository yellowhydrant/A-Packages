using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace A
{
    [AddComponentMenu(AConstants.ComponentMenuRoot + "/Signal Listener")]
    public class ASignalListener : MonoBehaviour
    {
        public ASignal signal;
        public UnityEvent SignalEvent;

        public void OnSignalRaised()
        {
            SignalEvent.Invoke();
        }

        void OnEnable()
        {
            signal.RegisterListener(this);
        }

        void OnDisable()
        {
            signal.DeregisterListener(this);
        }
    }
}
