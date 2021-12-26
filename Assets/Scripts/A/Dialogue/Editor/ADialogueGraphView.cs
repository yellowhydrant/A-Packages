#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace A.Dialogue.Editor
{
    public class ADialogueGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ADialogueGraphView, GraphView.UxmlTraits> { }

        public List<AExposedProperty> exposedProperties = new List<AExposedProperty>();
        public bool isClear;

        ADialogueGraph dialogueGraph;
        Blackboard blackboard;

        public ADialogueGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ADialogueConstants.StyleSheetPath);
            styleSheets.Add(styleSheet);

            InstantiateBlackboard();
        }

        //TODO: Add imguicontainer that displays charcater manifesto
        public void InstantiateBlackboard()
        {
            blackboard = new Blackboard(this);
            blackboard.Add(new BlackboardSection() { title = "Exposed Properties" });
            blackboard.addItemRequested = (bb) =>
            {
                AddPropertyToBlackboard(new AExposedProperty(),true);
            };
            blackboard.editTextRequested = (bb, elem, newValue) =>
            {
                var prevValue = ((BlackboardField)elem).text;
                if(exposedProperties.Any((x) => x.propertyName == newValue))
                {
                    Debug.LogError("This property name already exists! Please choose a different property name!");
                    return;
                }
                var propertyIndex = exposedProperties.FindIndex((x) => x.propertyName == prevValue);
                exposedProperties[propertyIndex].propertyName = newValue;
                ((BlackboardField)elem).text = newValue;
            };

            blackboard.SetPosition(new Rect(10, 30, 200, 300));

            Add(blackboard);
        }

        private void AddPropertyToBlackboard(AExposedProperty exposedProperty, bool addToDialogue)
        {
            //Create and male name locally unique
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

            //Create and add property to lists
            var property = new AExposedProperty();
            property.propertyName = localPropertyName;
            property.propertyValue = localPropertyValue;

            exposedProperties.Add(property);
            if(dialogueGraph != null && addToDialogue)
                dialogueGraph.exposedProperties.Add(property);

            //Add new blackboardField to the mainContainer
            var mainContainer = new VisualElement();
            var bbField = new BlackboardField() { text = property.propertyName, typeText = "Vector2"};
            mainContainer.Add(bbField);

            //Create and add valueField to propertyContainer
            var propertyContainer = new VisualElement();
            var valueField = new TextField("Value: ")
            {
                value = localPropertyValue
            };
            valueField[0].style.minWidth = 50f;
            valueField.RegisterValueChangedCallback((c) =>
            {
                property.propertyValue = c.newValue;
            });
            propertyContainer.Add(valueField);

            //Create and add isTagToggle to propertyContainer
            var isTagToggle = new Toggle() {label = "Is Tag:" };
            isTagToggle[0].style.minWidth = 158f;
            isTagToggle.RegisterValueChangedCallback((c) => property.isTag = c.newValue);
            propertyContainer.Add(isTagToggle);

            //Create and add a new BlackboardRow to mainContainer
            var blackboardValueRow = new BlackboardRow(bbField, propertyContainer);
            mainContainer.Add(blackboardValueRow);

            //Finally add the mainContainer to the blackboard
            blackboard.Add(mainContainer);
        } 

        public void PopulateView(ADialogueGraph dialogueGraph)
        {
            //Clean-up
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            blackboard.Clear();
            blackboard.Add(new BlackboardSection() { title = "Exposed Properties" });
            exposedProperties.Clear();
            isClear = true;

            //Assign graph
            this.dialogueGraph = dialogueGraph;
            if (dialogueGraph == null)
                return;

            //Create and add properties from data
            for (int i = 0; i < dialogueGraph.exposedProperties.Count; i++)
            {
                AddPropertyToBlackboard(dialogueGraph.exposedProperties[i], false);
            }

            //Create nodes from data
            InstantiateEntryPoint();
            LoadNodes();
            EditorApplication.delayCall += () => {
                FrameAll();
            };
            graphViewChanged += OnGraphViewChanged;
            isClear = false;
        }

        void InstantiateEntryPoint()
        {
            //Create new node
            var node = new ADialogueNode();
            node.title = "START";
            node.GUID = GUID.Generate().ToString();
            node.entryPoint = true;

            //Create entry port
            var port = GeneratePort(node, Direction.Output);
            port.portName = "next";
            node.outputContainer.Add(port);

            //Change node capabilities
            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            //Refresh and add to graphview
            RefreshNode(node);
            AddElement(node);
        }

        Port GeneratePort(ADialogueNode node, Direction dir, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, dir, capacity, typeof(string));
        }

        void RefreshNode(ADialogueNode node)
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

        public ADialogueNode CreateDialogueNode(ANodeData nodeData)
        {
            //Create node
            var node = new ADialogueNode();
            node.nodeData = nodeData;
            node.GUID = GUID.Generate().ToString();

            ////Remove node unfold toggle
            node.titleContainer.RemoveAt(0);
            node.titleButtonContainer.RemoveAt(0);

            //Create and add nameText
            var nameText = new Label();
            nameText.text = nodeData.speaker.name;
            nameText.style.unityTextAlign = TextAnchor.MiddleCenter;
            nameText.style.flexGrow = 1f;
            nameText.style.fontSize = 16f;
            nameText.style.unityFontStyleAndWeight = FontStyle.Bold;
            node.titleContainer.Add(nameText);

            //Create and add input port
            var input = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
            input.portName = "Input";
            node.inputContainer.Add(input);

            //Create and add new choice button
            var newChoiceButton = new Button(() => AddChoicePort(node));
            newChoiceButton.text = "New Choice";
            node.titleContainer.Add(newChoiceButton);

            if (!nodeData.isBranch)
            {
                //Create IMGUIContainer for additional nodeData variables
                var editor = UnityEditor.Editor.CreateEditor(node.nodeData);
                var container = new IMGUIContainer(() =>
                {
                    EditorGUILayout.BeginVertical();
                    if (dialogueGraph != null && dialogueGraph.register != null)
                    {
                        var names = new string[dialogueGraph.register.speakers.Count];
                        var cur = 0;
                        for (int i = 0; i < names.Length; i++)
                        {
                            names[i] = dialogueGraph.register.speakers[i].name;
                            if (dialogueGraph.register.speakers[i].guid == nodeData.speaker.guid)
                                cur = i;
                        }
                        nodeData.speaker = dialogueGraph.register.speakers[EditorGUILayout.Popup("Speaker:", cur, names)];
                        nameText.text = nodeData.speaker.name;
                    }
                    EditorGUILayout.EndVertical();
                    if (editor != null && editor.target != null)
                    {
                        editor.OnInspectorGUI();
                    }
                });
                container.style.maxWidth = 320f;
                node.mainContainer.Add(container);
            }
            else
            {
                node.TurnIntoBranchNode();
            }
            //Refresh and return node
            RefreshNode(node);
            return node;
        }

        public void CreateNode(Vector2 position = new Vector2())
        {
            //Create and add nodeData to asset
            var nodeData = ScriptableObject.CreateInstance<ANodeData>();
            nodeData.name = dialogueGraph.name + "_Node" + dialogueGraph.nodeData.Count;
            dialogueGraph.nodeData.Add(nodeData);
            AssetDatabase.AddObjectToAsset(nodeData, dialogueGraph);
            AssetDatabase.SaveAssets();

            //Create and add node to graphview
            var node = CreateDialogueNode(nodeData);
            nodeData.Init(node.GUID, "", position);
            node.SetPosition(new Rect(position, node.GetPosition().size));
            AddElement(node);
            AddChoicePort(node);
        }

        public void AddChoicePort(ADialogueNode node, string portName = null)
        {
            //Create and add port to node.outputContainer
            var port = GeneratePort(node, Direction.Output);
            var num = node.outputContainer.Query("connector").ToList().Count;
            if (num == 0)
                port.portName = portName == null ? ADialogueGraph.Continue : portName;
            else
                port.portName = portName == null ? $"Choice {num}" : portName;
            port.contentContainer.Remove(port.Q<Label>("type"));
            node.outputContainer.Add(port);

            //if (node.nodeData.isBranch)
            //{
            //    newValue = Regex.Replace(newValue, "[^0-9]", "");
            //    prevValue = Regex.Replace(prevValue, "[^0-9]", "");
            //}

            if (!node.nodeData.isBranch)
            {
                //Create and add choiceTextField to port.contentContainer
                var choiceTextField = new TextField() { name = string.Empty, value = port.portName };
                choiceTextField.RegisterValueChangedCallback((c) =>
                {
                    port.portName = c.newValue;
                    if (dialogueGraph != null)
                    {
                        var link = dialogueGraph.nodeLinks.FirstOrDefault((x) => x.baseGUID == node.GUID && x.portName == c.previousValue);
                        if (link != null)
                            link.portName = c.newValue;
                    }
                });
                port.contentContainer.Add(new Label("  "));
                port.contentContainer.Add(choiceTextField);
            }
            else
            {
                var choiceTextField = new IntegerField() { name = string.Empty, value = int.Parse(Regex.Replace(port.portName, "[^0-9]", "")) };
                choiceTextField.RegisterValueChangedCallback((c) =>
                {
                    port.portName = c.newValue.ToString();
                    if (dialogueGraph != null)
                    {
                        var link = dialogueGraph.nodeLinks.FirstOrDefault((x) => x.baseGUID == node.GUID && x.portName == c.previousValue.ToString());
                        if (link != null)
                            link.portName = c.newValue.ToString();
                    }
                });
                port.contentContainer.Add(new Label("  "));
                port.contentContainer.Add(choiceTextField);
            }

            //Create and add delete button
            var deleteBut = new Button(() => RemoveChoicePort(node, port)) { text = "x"};
            port.contentContainer.Add(deleteBut);

            RefreshNode(node);
        }

        private void RemoveChoicePort(ADialogueNode node, Port port)
        {
            //Get target edges
            var targetEdges = edges.ToList().Where((x) => x.output.portName == port.portName && x.output.node == port.node);

            //Check if any and get first edge
            if (!targetEdges.Any()) return;
            var edge = targetEdges.First();

            //Disconnect ports and remove edge from graphview
            edge.output.Disconnect(edge);
            edge.input.Disconnect(edge);
            RemoveElement(edge);

            //Remove links connected to this port
            var parentNode = edge.output.node as ADialogueNode;
            var childNode = edge.input.node as ADialogueNode;
            var linkToRemove = dialogueGraph.nodeLinks.First((link) => link.baseGUID == parentNode.GUID && link.targetGUID == childNode.GUID);
            dialogueGraph.nodeLinks.Remove(linkToRemove);

            //Remove port from node.outputContainer
            node.outputContainer.Remove(port);

            RefreshNode(node);
        }

        void LoadNodes()
        {
            //Check if there is anything to load
            if (!dialogueGraph.nodeData.Any()) return;

            //Get edges and nodes
            var edges = this.edges.ToList();
            var nodes = this.nodes.ToList().Cast<ADialogueNode>().ToList();

            //Get entryPoint
            nodes.Find((n) => n.entryPoint).GUID = dialogueGraph.nodeLinks[0].baseGUID;

            //Create and add nodes and their choice ports
            foreach (var nodeData in dialogueGraph.nodeData)
            {
                var newNode = CreateDialogueNode(nodeData);
                newNode.GUID = nodeData.GUID;
                newNode.SetPosition(new Rect(nodeData.nodePosition, newNode.GetPosition().size));
                AddElement(newNode);

                var nodePorts = dialogueGraph.nodeLinks.Where((x) => x.baseGUID == nodeData.GUID).ToList();
                nodePorts.ForEach((x) => AddChoicePort(newNode, x.portName));
            }

            //Re-get nodes
            nodes = this.nodes.ToList().Cast<ADialogueNode>().ToList();

            //Connect all nodes
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
            //Create and add new edge
            var newEdge = new Edge() { output = output, input = input };
            newEdge.input.Connect(newEdge);
            newEdge.output.Connect(newEdge);
            AddElement(newEdge);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //Get click position
            var position = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
            evt.menu.AppendAction("Create Node", (d) => CreateNode(position));
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (dialogueGraph == null)
                return graphViewChange;

            //Remove and disconnect nodes and edges from asset and graphview
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var node = elem as ADialogueNode;
                    if (node != null)
                    {
                        var nodeData = dialogueGraph.nodeData.First((nd) => nd.GUID == node.GUID);
                        dialogueGraph.nodeData.Remove(nodeData);
                        AssetDatabase.RemoveObjectFromAsset(nodeData);
                        //Object.DestroyImmediate(nodeData);
                        AssetDatabase.SaveAssets();
                    }

                    var edge = elem as Edge;
                    if (edge != null)
                    {
                        var parentNode = edge.output.node as ADialogueNode;
                        var childNode = edge.input.node as ADialogueNode;
                        var linkToRemove = dialogueGraph.nodeLinks.First((link) => link.baseGUID == parentNode.GUID && link.targetGUID == childNode.GUID);
                        dialogueGraph.nodeLinks.Remove(linkToRemove);
                    }
                });
            }
            //Connect and add edges to asset and graphview
            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    
                    var parentNode = edge.output.node as ADialogueNode;
                    var childNode = edge.input.node as ADialogueNode;
                    var nodeData = new ADialogueGraph.LinkData(parentNode.GUID, edge.output.portName, childNode.GUID);
                    if (!dialogueGraph.nodeLinks.Contains(nodeData))
                        dialogueGraph.nodeLinks.Add(nodeData);
                });
            }

            //Update positions of nodes in nodeData
            if(graphViewChange.movedElements != null)
            {
                graphViewChange.movedElements.ForEach(elem =>
                {
                    var node = elem as ADialogueNode;
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