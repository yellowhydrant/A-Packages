using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.BehaviourTree.Nodes {
    public abstract class ADecoratorNode : ANode {
        [HideInInspector] public ANode child;

        public override ANode Clone() {
            ADecoratorNode node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}
