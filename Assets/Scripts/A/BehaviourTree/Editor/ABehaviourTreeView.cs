#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using A.BehaviourTree.Nodes;

namespace A.BehaviourTree {
    //TODO: Get rid of behaviourtree settings
    public class ABehaviourTreeView : GraphView {

        public Action<ANodeView> OnNodeSelected;
        public new class UxmlFactory : UxmlFactory<ABehaviourTreeView, GraphView.UxmlTraits> { }
        ABehaviourTree tree;

        public ABehaviourTreeView() 
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new DoubleClickSelection());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ABehaviourTreeConstants.TreeStyleSheetPath);
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;
        }

        private void OnUndoRedo() 
        {
            PopulateView(tree);
            AssetDatabase.SaveAssets();
        }

        public ANodeView FindNodeView(ANode node) 
        {
            return GetNodeByGuid(node.guid) as ANodeView;
        }

        internal void PopulateView(ABehaviourTree tree) 
        {
            this.tree = tree;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            graphViewChanged += OnGraphViewChanged;

            if (tree == null)
                return;

            if (tree.rootNode == null) {
                tree.rootNode = tree.CreateNode(typeof(ARootNode)) as ARootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            // Creates node view
            tree.nodes.ForEach(n => CreateNodeView(n));

            // Create edges
            tree.nodes.ForEach(n => {
                var children = ABehaviourTree.GetChildren(n);
                children.ForEach(c => {
                    ANodeView parentView = FindNodeView(n);
                    ANodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) 
        {
            return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange) 
        {
            if (graphViewChange.elementsToRemove != null) {
                graphViewChange.elementsToRemove.ForEach(elem => {
                    ANodeView nodeView = elem as ANodeView;
                    if (nodeView != null) {
                        tree.DeleteNode(nodeView.node);
                    }

                    Edge edge = elem as Edge;
                    if (edge != null) {
                        ANodeView parentView = edge.output.node as ANodeView;
                        ANodeView childView = edge.input.node as ANodeView;
                        tree.RemoveChild(parentView.node, childView.node);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null) {
                graphViewChange.edgesToCreate.ForEach(edge => {
                    ANodeView parentView = edge.output.node as ANodeView;
                    ANodeView childView = edge.input.node as ANodeView;
                    tree.AddChild(parentView.node, childView.node);
                });
            }

            nodes.ForEach((n) => {
                ANodeView view = n as ANodeView;
                view.SortChildren();
            });

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt) 
        {
            Vector2 nodePosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            
            AppendMenuFor<AActionNode>();
            AppendMenuFor<ACompositeNode>();
            AppendMenuFor<ADecoratorNode>();

            void AppendMenuFor<T>()
            {
                var types = TypeCache.GetTypesDerivedFrom<T>();
                foreach (var type in types)
                {
                    var path = $"[{type.Namespace.Substring(typeof(T).Namespace.Length).Substring(1)}]";
                    var lowestType = type;
                    var hierarchy = new List<Type>();
                    while (lowestType != typeof(T))
                    {
                        lowestType = lowestType.BaseType;
                        hierarchy.Add(lowestType);
                    }
                    for (int i = hierarchy.Count - 1; i >= 0; i--)
                    {
                        path += $"/[{hierarchy[i].Name.Substring(1)}{(lowestType == typeof(T) ? "s" : "")}]";
                    }
                    evt.menu.AppendAction($"{path}/{type.Name.Substring(1)}", (a) => CreateNode(type, nodePosition));
                }
            }
        }

        void CreateNode(System.Type type, Vector2 position) 
        {
            ANode node = tree.CreateNode(type);
            node.position = position;
            CreateNodeView(node);
        }

        void CreateNodeView(ANode node) 
        {
            ANodeView nodeView = new ANodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }

        public void UpdateNodeStates() 
        {
            nodes.ForEach(n => {
                ANodeView view = n as ANodeView;
                view.UpdateState();
            });
        }
    }
}
#endif