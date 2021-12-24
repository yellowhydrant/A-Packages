#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace A.Linking.Editor
{
    public class ALinkView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<ALinkView, GraphView.UxmlTraits> { }

        public ALinkView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/A/Linking/Editor/LinkEditor.uss");
            styleSheets.Add(styleSheet);
        }

        public void PopulateView()
        {
            var linkables = Object.FindObjectsOfType<MonoBehaviour>().OfType<ILinkable>().ToArray();

            if (linkables == null || linkables.Length == 0)
                return;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements.ToList());
            LoadNodes(linkables);
            graphViewChanged += OnGraphViewChanged;
        }

        void LoadNodes(ILinkable[] linkables)
        {
            for (int i = 0; i < linkables.Length; i++)
            {
                CreateLinkNodes(linkables[i]);
            }

            var nodes = this.nodes.ToList().Cast<ALinkNode>().ToList();
            for (int x = 0; x < nodes.Count; x++)
            {
                for (int y = 0; y < nodes[x].link.outputGUIDs.Length; y++)
                {
                    var outputGuid = nodes[x].link.outputGUIDs[y];
                    var output = nodes[x].outputContainer.Query<Port>().ToList().First((p) => p.portName == outputGuid);
                    var inputNodes = nodes.Where((n) => n.link.inputGUIDs.Any((guid) => guid == outputGuid)).ToList();
                    for (int z = 0; z < inputNodes.Count; z++)
                    {
                        var inputs = inputNodes[z].inputContainer.Query<Port>().Where((p) => (p.node as ALinkNode).link.inputGUIDs.Contains(outputGuid)).ToList();
                        for (int w = 0; w < inputs.Count; w++)
                        {
                            LinkNodes(output, inputs[w]);
                        }
                    }
                }

                for (int y = 0; y < nodes[x].link.oldOutputGUIDs.Count; y++)
                {
                    var outputGuid = nodes[x].link.oldOutputGUIDs[y];
                    var output = nodes[x].outputContainer.Query<Port>().ToList().First((p) => p.portName == outputGuid);
                    var inputNodes = nodes.Where((n) => n.link.inputGUIDs.Any((guid) => guid == outputGuid)).ToList();
                    for (int z = 0; z < inputNodes.Count; z++)
                    {
                        var inputs = inputNodes[z].inputContainer.Query<Port>().Where((p) => (p.node as ALinkNode).link.inputGUIDs.Contains(outputGuid)).ToList();
                        for (int w = 0; w < inputs.Count; w++)
                        {
                            LinkNodes(output, inputs[w]);
                        }
                    }
                    if (!output.connected)
                    {
                        nodes[x].link.oldOutputGUIDs.Remove(output.portName);
                        nodes[x].outputContainer.Remove(output);
                    }
                }
            }
        }

        public void CreateLinkNodes(ILinkable linkable)
        {
            var group = new ALinkNodeGroup();
            group.title = (linkable as MonoBehaviour).name;
            group.GUID = System.Guid.NewGuid().ToString();
            group.linkable = linkable;

            for (int x = 0; x < linkable.links.Length; x++)
            {
                var link = linkable.links[x];
                var node = new ALinkNode();
                node.title = link.name;
                node.group = group;
                node.link = link;

                AddPort(node, null, Direction.Input);
                for (int y = 0; y < link.outputGUIDs.Length; y++)
                {
                    AddPort(node, link.outputGUIDs[y], Direction.Output);
                }

                for (int z = 0; z < link.oldOutputGUIDs.Count; z++)
                {
                    var port = AddPort(node, link.oldOutputGUIDs[z], Direction.Output);
                    port.portColor = Color.red;
                }

                var position = link.nodePosition;
                //FIXME: Align elements in group
                //if (position == Vector2.zero && x != 0)
                //{
                //    var prevNodeRect = (group.containedElements.Last() as LinkNode).GetPosition();
                //    position = prevNodeRect.position - new Vector2(0, prevNodeRect.size.y);
                //    Debug.Log(prevNodeRect);
                //}

                AddElement(node);
                group.AddElement(node);

                RefreshNode(node);
                node.capabilities &= ~Capabilities.Deletable;

                node.SetPosition(new Rect(position, node.GetPosition().size));
            }

            group.SetPosition(new Rect(linkable.groupPosition, group.GetPosition().size));
            group.capabilities &= ~Capabilities.Deletable;
            AddElement(group);
        }

        private Edge LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge() { output = output, input = input };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            AddElement(tempEdge);
            return tempEdge;
        }

        Port GeneratePort(ALinkNode node, Direction dir, Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, dir, capacity, typeof(Vector2));
        }

        void RefreshNode(ALinkNode node)
        {
            node.RefreshExpandedState();
            node.RefreshPorts();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endport =>
            endport.direction != startPort.direction &&
            endport.node != startPort.node && startPort.portColor != Color.red && endport.portColor != Color.red).ToList();
        }

        public Port AddPort(ALinkNode node, string portValue, Direction direction)
        {
            var port = GeneratePort(node, direction, Port.Capacity.Multi);
            port.portName = portValue;
            var portName = "";
            if (direction == Direction.Output)
            {
                portName = "Out " + node.outputContainer.Query("connector").ToList().Count.ToString();
                node.outputContainer.Add(port);
            }
            else
            {
                portName = "In";
                node.inputContainer.Add(port);
            }

            port.contentContainer.Remove(port.Q<Label>("type"));

            var text = new TextElement() { text = portName };

            port.contentContainer.Add(text);

            RefreshNode(node);
            return port;
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                graphViewChange.elementsToRemove.ForEach(elem =>
                {
                    var edge = elem as Edge;
                    if (edge != null)
                    {
                        var childNode = edge.input.node as ALinkNode;
                        childNode.link.inputGUIDs.Remove(edge.output.portName);
                        if (edge.output.portColor == Color.red && edge.output.connections.ToList().Count <= 1)
                        {
                            var parentNode = edge.output.node as ALinkNode;
                            parentNode.link.oldOutputGUIDs.Remove(edge.output.portName);
                            edge.output.node.outputContainer.Remove(edge.output);
                            RefreshNode(parentNode);
                        }
                    }
                });
            }

            if (graphViewChange.edgesToCreate != null)
            {
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    var node = edge.input.node as ALinkNode;
                    node.link.inputGUIDs.Add(edge.output.portName);
                });
            }

            if (graphViewChange.movedElements != null)
            {
                graphViewChange.movedElements.ForEach(elem =>
                {
                    var node = elem as ALinkNode;
                    if (node != null)
                    {
                        node.link.nodePosition = node.GetPosition().position;
                    }
                    var group = elem as ALinkNodeGroup;
                    if (group != null)
                    {
                        group.linkable.groupPosition = group.GetPosition().position;
                    }
                });
            }

            return graphViewChange;
        }
    }
}
#endif