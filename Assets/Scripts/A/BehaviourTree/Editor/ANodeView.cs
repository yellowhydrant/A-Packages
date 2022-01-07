#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using A.BehaviourTree.Nodes;

namespace A.BehaviourTree
{

    public class ANodeView : UnityEditor.Experimental.GraphView.Node {
        public Action<ANodeView> OnNodeSelected;
        public ANode node;
        public Port input;
        public Port output;

        public ANodeView(ANode node) : base(AssetDatabase.GetAssetPath(BehaviourTreeSettings.GetOrCreateSettings().nodeXml)) {
            this.node = node;
            this.node.name = node.GetType().Name.Substring(1);
            this.title = node.name.Replace("(Clone)", "").Replace("Node", "");
            this.viewDataKey = node.guid;

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetupClasses();
            SetupDataBinding();
        }

        private void SetupDataBinding() {
            TextField descriptionField = this.Q<TextField>("description");
            descriptionField.value = node.description;
            descriptionField.RegisterValueChangedCallback((ctx) => node.description = ctx.newValue);
            //descriptionLabel.bindingPath = "description";
            //descriptionLabel.Bind(new SerializedObject(node));
        }

        private void SetupClasses() {
            if (node is AActionNode) {
                AddToClassList("action");
            } else if (node is ACompositeNode) {
                AddToClassList("composite");
            } else if (node is ADecoratorNode) {
                AddToClassList("decorator");
            } else if (node is ARootNode) {
                AddToClassList("root");
            }
        }

        private void CreateInputPorts() {
            if (node is AActionNode) {
                input = new ANodePort(Direction.Input, Port.Capacity.Single);
            } else if (node is ACompositeNode) {
                input = new ANodePort(Direction.Input, Port.Capacity.Single);
            } else if (node is ADecoratorNode) {
                input = new ANodePort(Direction.Input, Port.Capacity.Single);
            } else if (node is ARootNode) {

            }

            if (input != null) {
                input.portName = "";
                input.style.flexDirection = FlexDirection.Column;
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts() {
            if (node is AActionNode) {

            } else if (node is ACompositeNode) {
                output = new ANodePort(Direction.Output, Port.Capacity.Multi);
            } else if (node is ADecoratorNode) {
                output = new ANodePort(Direction.Output, Port.Capacity.Single);
            } else if (node is ARootNode) {
                output = new ANodePort(Direction.Output, Port.Capacity.Single);
            }

            if (output != null) {
                output.portName = "";
                output.style.flexDirection = FlexDirection.ColumnReverse;
                outputContainer.Add(output);
            }
        }

        public override void SetPosition(Rect newPos) {
            base.SetPosition(newPos);
            Undo.RecordObject(node, "Behaviour Tree (Set Position");
            node.position.x = newPos.xMin;
            node.position.y = newPos.yMin;
            EditorUtility.SetDirty(node);
        }

        public override void OnSelected() {
            base.OnSelected();
            if (OnNodeSelected != null) {
                OnNodeSelected.Invoke(this);
            }
        }

        public void SortChildren() {
            if (node is ACompositeNode composite) {
                composite.children.Sort(SortByHorizontalPosition);
            }
        }

        private int SortByHorizontalPosition(ANode left, ANode right) {
            return left.position.x < right.position.x ? -1 : 1;
        }

        public void UpdateState() {

            RemoveFromClassList("running");
            RemoveFromClassList("failure");
            RemoveFromClassList("success");

            if (Application.isPlaying) {
                switch (node.state) {
                    case ANode.State.Running:
                        if (node.started) {
                            AddToClassList("running");
                        }
                        break;
                    case ANode.State.Failure:
                        AddToClassList("failure");
                        break;
                    case ANode.State.Success:
                        AddToClassList("success");
                        break;
                }
            }
        }
    }
}
#endif