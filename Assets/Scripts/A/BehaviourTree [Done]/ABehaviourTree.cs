using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using A.BehaviourTree.Nodes;


namespace A.BehaviourTree {
    [CreateAssetMenu(menuName = AConstants.AssetMenuRoot + "/" + ABehaviourTreeConstants.AssetMenuRoot + "/Behaviour Tree")]
    public class ABehaviourTree : ScriptableObject {
        public ANode rootNode;
        public ANode.State treeState = ANode.State.Running;
        public List<ANode> nodes = new List<ANode>();
        public ABlackboard blackboard = new ABlackboard();

        public ANode.State Update() {
            if (rootNode.state == ANode.State.Running) {
                treeState = rootNode.Update();
            }
            return treeState;
        }

        public static List<ANode> GetChildren(ANode parent) {
            List<ANode> children = new List<ANode>();

            if (parent is ADecoratorNode decorator && decorator.child != null) {
                children.Add(decorator.child);
            }

            if (parent is ARootNode rootNode && rootNode.child != null) {
                children.Add(rootNode.child);
            }

            if (parent is ACompositeNode composite) {
                return composite.children;
            }

            return children;
        }

        public static void Traverse(ANode node, System.Action<ANode> visiter) {
            if (node) {
                visiter.Invoke(node);
                var children = GetChildren(node);
                children.ForEach((n) => Traverse(n, visiter));
            }
        }

        public ABehaviourTree Clone() {
            ABehaviourTree tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<ANode>();
            Traverse(tree.rootNode, (n) => {
                tree.nodes.Add(n);
            });

            return tree;
        }

        public void Bind(AContext context) {
            Traverse(rootNode, node => {
                node.context = context;
                node.blackboard = blackboard;
            });
        }


        #region Editor Compatibility
#if UNITY_EDITOR

        public ANode CreateNode(System.Type type) {
            ANode node = ScriptableObject.CreateInstance(type) as ANode;
            node.name = type.Name;
            node.guid = GUID.Generate().ToString();

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");
            nodes.Add(node);

            if (!Application.isPlaying) {
                AssetDatabase.AddObjectToAsset(node, this);
            }

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();
            return node;
        }

        public void DeleteNode(ANode node) {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
            nodes.Remove(node);

            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(ANode parent, ANode child) {
            if (parent is ADecoratorNode decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");
                decorator.child = child;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is ARootNode rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree (AddChild)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is ACompositeNode composite) {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");
                composite.children.Add(child);
                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(ANode parent, ANode child) {
            if (parent is ADecoratorNode decorator) {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");
                decorator.child = null;
                EditorUtility.SetDirty(decorator);
            }

            if (parent is ARootNode rootNode) {
                Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
                rootNode.child = null;
                EditorUtility.SetDirty(rootNode);
            }

            if (parent is ACompositeNode composite) {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");
                composite.children.Remove(child);
                EditorUtility.SetDirty(composite);
            }
        }
#endif
        #endregion Editor Compatibility
    }
}