using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.BehaviourTree {
    public class AInterruptSelector : ASelector {
        protected override State OnUpdate() {
            int previous = current;
            base.OnStart();
            var status = base.OnUpdate();
            if (previous != current) {
                if (children[previous].state == State.Running) {
                    children[previous].Abort();
                }
            }

            return status;
        }
    }
}