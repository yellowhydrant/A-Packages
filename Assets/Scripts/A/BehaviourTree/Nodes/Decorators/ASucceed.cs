using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.BehaviourTree {
    public class ASucceed : ADecoratorNode {
        protected override void OnStart() {
        }

        protected override void OnStop() {
        }

        protected override State OnUpdate() {
            var state = child.Update();
            if (state == State.Failure) {
                return State.Success;
            }
            return state;
        }
    }
}