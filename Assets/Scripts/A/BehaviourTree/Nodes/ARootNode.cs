using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.BehaviourTree {

    public class ARootNode : ANode {
        public ANode child;

        protected override void OnStart() {

        }

        protected override void OnStop() {

        }

        protected override State OnUpdate() {
            return child.Update();
        }

        public override ANode Clone() {
            ARootNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}