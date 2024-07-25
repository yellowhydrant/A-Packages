using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A.BehaviourTree {
    public class ABehaviourTreeRunner : MonoBehaviour {

        // The main behaviour tree asset
        public ABehaviourTree tree;

        // Storage container object to hold game object subsystems
        AContext context;

        // Start is called before the first frame update
        void Start() {
            context = CreateBehaviourTreeContext();
            tree = tree.Clone();
            tree.Bind(context);
        }

        // Update is called once per frame
        void Update() {
            if (tree) {
                tree.Update();
            }
        }

        AContext CreateBehaviourTreeContext() {
            return AContext.CreateFromGameObject(gameObject);
        }

        private void OnDrawGizmosSelected() {
            if (!tree) {
                return;
            }

            ABehaviourTree.Traverse(tree.rootNode, (n) => {
                if (n.drawGizmos) {
                    n.OnDrawGizmos();
                }
            });
        }
    }
}