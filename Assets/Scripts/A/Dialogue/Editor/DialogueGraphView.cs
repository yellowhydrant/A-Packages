#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;


namespace A.Dialogue.Editor
{
    public class DialogueGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<DialogueGraphView, GraphView.UxmlTraits> { }

        Blackboard blackboard;
        public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
        public bool isClear;
        ADialogueGraph dialogueGraph;

        public DialogueGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/A/Dialogue/Editor/DialogueGraphEditorStyle.uss");
            styleSheets.Add(styleSheet);

            InstantiateBlackboard();
        }

        public void InstantiateBlackboard()
        {
            blackboard = new Blackboard(this);
            blackboard.Add(new BlackboardSection() { title = "Exposed Properties" });
            blackboard.addItemRequested = (b) =>
            {
                AddPropertyToBlackboard(new ExposedProperty(),true);
            };
            blackboard.editTextRequested = (bb, elem, newValue) =>
            {
                var prevValue = ((BlackboardField)elem).text;
                if(exposedProperties.Any((x) => x.propertyName == newValue))
                {
                    Debug.LogError("This property name already exists! Please choose a different and UNIQUE property name!");
                    return;
                }
                var propertyIndex = exposedProperties.FindIndex((x) => x.propertyName == prevValue);
                exposedProperties[propertyIndex].propertyName = newValue;
                ((BlackboardField)elem).text = newValue;
            };

            blackboard.SetPosition(new Rect(10, 30, 200, 300));

            Add(blackboard);
        }

        private void AddPropertyToBlackboard(ExposedProperty exposedProperty, bool addToNarrative)
        {
            var localPropertyName = exposedProperty.propertyName;
            var localPropertyValue = exposedProperty.propertyValue;
            var num = 1;
            var prevSufix = "";
            var sufix = "";
            while (exposedProperties.Any((x) => x.propertyName == localPropertyName))
            {
                prevSufix = sufix;
                sufix = $"({num})";
                if (prevSufix == "")
                    localPropertyName = $"{localPropertyName}" + sufix;
                else
                    localPropertyName = localPropertyName.Replace(prevSufix, sufix);
                num++;
            }

            var property = new ExposedProperty();
            property.propertyName = localPropertyName;
            property.propertyValue = localPropertyValue;

            exposedProperties.Add(property);
            if(dialogueGraph != null && addToNarrative)
                dialogueGraph.exposedProperties.Add(property);

            var mainContainer = new VisualElement();

            var propertyContainer = new VisualElement();

            var field = new BlackboardField() { text = property.propertyName, typeText = "Vector2"};
            mainContainer.Add(field);

            var propertyValueField = new TextField("Value: ")
            {
                value = localPropertyValue
            };
            propertyValueField[0].style.minWidth = 50f;
            propertyValueField.RegisterValueChangedCallback((c) =>
            {
                property.propertyValue = c.newValue;
            });


            propertyContainer.Add(propertyValueField);

            var blackboardValueRow = new BlackboardRow(field, propertyContainer);
            mainContainer.Add(blackboardValueRow);

            var toggle = new Toggle() {label = "Is Tag:" };
            toggle[0].style.minWidth = 158f;
            toggle.RegisterValueChangedCallback((c) => property.isTag = c.newValue);

            propertyContainer.Add(toggle);

            blackboard.Add(mainContainer);
        }

        //public void InstantiateMinimap()
        //{
        //    var map = new MiniMap() { anchored = true };
        //    var cords = contentViewContainer.WorldToLocal(new Vector2(Window.maxSize.x - 10, 30));

        //    map.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        //    map.RemoveAt(0);
        //    Add(map);
        //}

        public void PopulateView(ADialogueGraph dialogueGraph)
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            blackboard.Clear();
            blackboard.Add(new BlackboardSection() { title = "Exposed Properties" });

            exposedProperties.Clear();

            isClear = true;

            this.dialogueGraph = dialogueGraph;

            if (dialogueGraph == null)
                return;

            for (int i = 0; i < dialogueGraph.exposedProperties.Count; i++)
            {
                AddPropertyToBlackboard(dialogueGraph.exposedProperties[i], false);
            }

            InstantiateEntryPoint();
            LoadNodes();
            graphViewChanged += OnGraphViewChanged;
            isClear = false;
        }

        void InstantiateEntryPoint()
        {
            var node = new DialogueNode();
            node.title = "START";
            node.GUID = GUID.Generate().ToString();
            //node.speakerDialogue = "";
            node.entryPoint = true;

            var port = GeneratePort(node, Direction.Output);
            port.portName = "next";
            node.outputContainer.Add(port);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            RefreshNode(node);

            AddElement(node);
        }

        Port GeneratePort(DialogueNode node, Direction dir, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, dir, capacity, typeof(string));
        }

        void RefreshNode(DialogueNode node)
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endport =>
            endport.direction != startPort.direction &&
            endport.node != startPort.node).ToList();
        }

        public DialogueNode CreateNarrativeNode(NodeData nodeData)
        {
            var node = new DialogueNode();
            //node.speakerDialogue = nodeData.speakerDialogue;
            //node.speakerName = nodeData.speakerName;
            node.nodeData = nodeData;
            node.GUID = GUID.Generate().ToString();

            node.titleContainer.RemoveAt(0);
            node.titleButtonContainer.RemoveAt(0);
            var nameField = new TextField();
            nameField.style.flexGrow = 1f;
            nameField.RegisterValueChangedCallback((c) =>
            {
                //node.speakerName = c.newValue;
                node.nodeData.speakerName = c.newValue;
                //if (dialogueGraph != null)
                //    dialogueGraph.nodeData.First((x) => x.GUID == node.GUID).speakerName = node.speakerName;
            });
            nameField.SetValueWithoutNotify(nodeData.speakerName);
            node.titleContainer.Add(nameField);

            var input = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            input.portName = "Input";
            node.inputContainer.Add(input);

            var button = new Button(() => AddChoicePort(node));
            button.text = "New Choice";
            node.titleContainer.Add(button);

            var editor = UnityEditor.Editor.CreateEditor(node.nodeData);
            var container = new IMGUIContainer(() =>
            {
                if (editor.target != null)
                {
                    editor.OnInspectorGUI();
                }
            });
            container.style.maxWidth = 320f;
            node.mainContainer.Add(container);

            RefreshNode(node);
            return node;
        }

        public void CreateNode(Vector2 position = new Vector2())
        {
            var nodeData = ScriptableObject.CreateInstance<NodeData>();
            nodeData.name = dialogueGraph.name + "_Node" + dialogueGraph.nodeData.Count;
            dialogueGraph.nodeData.Add(nodeData);
            AssetDatabase.AddObjectToAsset(nodeData, dialogueGraph);
            AssetDatabase.SaveAssets();

            var node = CreateNarrativeNode(nodeData);
            nodeData.Init(node.GUID, "", position);
            node.SetPosition(new Rect(position, node.GetPosition().size));
            AddElement(node);
            AddChoicePort(node);
        }

        public void AddChoicePort(DialogueNode node, string portName = null)
        {
            var port = GeneratePort(node, Direction.Output);
            var num = node.outputContainer.Query("connector").ToList().Count;
            if(num == 0)
                port.portName = portName == null ? ADialogueParser.Continue : portName;
            else
                port.portName = portName == null ? $"Choice {num}" : portName;
            node.outputContainer.Add(port);

            port.contentContainer.Remove(port.Q<Label>("type"));

            var textField = new TextField() { name = string.Empty, value = port.portName };
            textField.RegisterValueChangedCallback((c) =>
            {
                port.portName = c.newValue;
                //node.speakerDialogue = c.newValue;
                if (dialogueGraph != null)
                {
                    var link = dialogueGraph.nodeLinks.FirstOrDefault((x) => x.baseGUID == node.GUID && x.portName == c.previousValue);
                    if(link != null)
                        link.portName = c.newValue;
                }
            });
            port.contentContainer.Add(new Label("  "));
            port.contentContainer.Add(textField);

            var deleteBut = new Button(() => RemoveChoicePort(node, port)) { text = "x"};
            port.contentContainer.Add(deleteBut);

            RefreshNode(node);
        }

        private void RemoveChoicePort(DialogueNode node, Port port)
        {
            var targetEdges = edges.ToList().Where((x) => x.output.portName == port.portName && x.output.node == port.node);

            if (!targetEdges.Any()) return;
            var edge = targetEdges.First();

            ////narrative.nodeLinks.Remove(new DialogueGraph.nodeLinks(node.GUID, port.portName, (edge.input.node as NarrativeNode).GUID));
            //if (dialogueGraph != null)
            //    dialogueGraph.nodeLinks = dialogueGraph.nodeLinks.Where((x) => x.baseGUID != node.GUID && x.targetGUID != (edge.input.node as DialogueNode).GUID).ToList();

            edge.output.Disconnect(edge);
            edge.input.Disconnect(edge);
            RemoveElement(edge);

            var parentNode = edge.output.node as DialogueNode;
            var childNode = edge.input.node as DialogueNode;
            var linkToRemove = dialogueGraph.nodeLinks.First((link) => link.baseGUID == parentNode.GUID && link.targetGUID == childNode.GUID);
            dialogueGraph.nodeLinks.Remove(linkToRemove);

            node.outputContainer.Remove(port);

            RefreshNode(node);
        }

        void LoadNodes()
        {
            var edges = this.edges.ToList();
            var nodes = this.nodes.ToList().Cast<DialogueNode>().ToList();

            if (!dialogueGraph.nodeData.Any()) return;

            nodes.Find((n) => n.entryPoint).GUID = dialogueGraph.nodeLinks[0].baseGUID;

            foreach (var nodeData in dialogueGraph.nodeData)
            {
                var tempNode = CreateNarrativeNode(nodeData);
                tempNode.GUID = nodeData.GUID;
                tempNode.SetPosition(new Rect(nodeData.nodePosition, tempNode.GetPosition().size));
                AddElement(tempNode);

                var nodePorts = dialogueGraph.nodeLinks.Where((x) => x.baseGUID == nodeData.GUID).ToList();
                nodePorts.ForEach((x) => AddChoicePort(tempNode, x.portName));
            }

            nodes = this.nodes.ToList().Cast<DialogueNode>().ToList();

            for (int i = 0; i < nodes.Count; i++)
            {
                var connections = dialogueGraph.nodeLinks.Where((x) => x.baseGUID == nodes[i].GUID).ToList();
                for (int j = 0; j < connections.Count; j++)
                {
                    var targetGuid = connections[j].targetGUID;
                    var targetNode = nodes.First((x) => x.GUID == targetGuid);
                    LinkNodes(nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);
                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge() { output = output, input = input };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            AddElement(tempEdge);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            evt.menu.AppendAction("Create Node", (d) => CreateNode(position));
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (dialogueGraph == null)
                return graphViewChange;

            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var node = elem as DialogueNode;
                    if (node != null)
                    {
                        var nodeData = dialogueGraph.nodeData.First((nd) => nd.GUID == node.GUID);
                        dialogueGraph.nodeData.Remove(nodeData);
                        AssetDatabase.RemoveObjectFromAsset(nodeData);
                        AssetDatabase.SaveAssets();
                    }

                    var edge = elem as Edge;
                    if (edge != null)
                    {
                        var parentNode = edge.output.node as DialogueNode;
                        var childNode = edge.input.node as DialogueNode;
                        var linkToRemove = dialogueGraph.nodeLinks.First((link) => link.baseGUID == parentNode.GUID && link.targetGUID == childNode.GUID);
                        dialogueGraph.nodeLinks.Remove(linkToRemove);
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    
                    var parentNode = edge.output.node as DialogueNode;
                    var childNode = edge.input.node as DialogueNode;
                    var nodeData = new ADialogueGraph.LinkData(parentNode.GUID, edge.output.portName, childNode.GUID);
                    if (!dialogueGraph.nodeLinks.Contains(nodeData))
                        dialogueGraph.nodeLinks.Add(nodeData);
                });
            }

            if(graphViewChange.movedElements != null)
            {
                graphViewChange.movedElements.ForEach(elem =>
                {
                    var node = elem as DialogueNode;
                    if(node != null)
                    {
                        dialogueGraph.nodeData.First((x) => x.GUID == node.GUID).nodePosition = node.GetPosition().position;
                    }
                });
            }

            return graphViewChange;
        }
    }
}
#endif