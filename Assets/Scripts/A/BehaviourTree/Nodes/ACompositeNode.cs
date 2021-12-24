using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.BehaviourTree {
    public abstract class ACompositeNode : ANode {
        [HideInInspector] public List<ANode> children = new List<ANode>();

        public override ANode Clone() {
            ACompositeNode node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}