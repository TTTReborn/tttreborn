using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : LibraryAttribute
    {
        public NodeAttribute(string name) : base("node_" + name)
        {

        }
    }

    public abstract class Node : Modal
    {
        public string LibraryName { get; set; }
        public List<Node> NextNodes { get; set; } = new();
        public List<int> ConnectPositions { get; set; } = new();

        public List<NodeSetting> NodeSettings = new();
        public StackNode StackNode;

        public Node(StackNode stackNode) : base()
        {
            StackNode = stackNode;
            LibraryName = Utils.GetLibraryName(GetType());

            Header.DragHeader.IsFreeDraggable = true;
            Header.DragHeader.IsLocked = false;

            StyleSheet.Load("/ui/visual-programming/nodes/node/Node.scss");

            AddClass("node");
            AddClass("box-shadow");
        }

        public static NodeAttribute GetAttribute<T>() where T : Node => Library.GetAttribute(typeof(T)) as NodeAttribute;

        public NodeAttribute GetAttribute() => Library.GetAttribute(GetType()) as NodeAttribute;

        public T AddSetting<T>() where T : NodeSetting, new()
        {
            T nodeSetting = new();
            nodeSetting.Node = this;

            Content.AddChild(nodeSetting);
            NodeSettings.Add(nodeSetting);

            return nodeSetting;
        }

        private Node GetConnectedNode(NodeConnectionPoint connectionPoint, out int index)
        {
            index = 0;

            NodeConnectionWire connectionWire = connectionPoint.ConnectionWire;

            if (connectionWire == null)
            {
                return null;
            }

            NodeConnectionPoint connectedPoint = connectionWire.EndPoint;

            if (connectedPoint == null)
            {
                return null;
            }

            index = connectedPoint.GetSettingsIndex();

            return connectedPoint.Node;
        }

        public virtual void Build(params object[] input)
        {
            NextNodes.Clear();

            for (int i = 0; i < NodeSettings.Count; i++)
            {
                NodeSetting nodeSetting = NodeSettings[i];

                if (nodeSetting.Output == null)
                {
                    continue;
                }

                Node connectedNode = GetConnectedNode(nodeSetting.Output.ConnectionPoint, out int connectPositionIndex);

                if (connectedNode == null)
                {
                    continue;
                }

                NextNodes.Add(connectedNode);
                ConnectPositions.Add(connectPositionIndex);
                StackNode.NextNodes.Add(connectedNode.StackNode);
            }

            object[] arr = null;

            try
            {
                arr = StackNode.Build(input);
            }
            catch (Exception e)
            {
                HighlightError();

                if (e is NodeStackException)
                {
                    Log.Warning($"Error in note '{GetType()}': ({e.Source}): {e.Message}\n{e.StackTrace}");
                }
                else
                {
                    Log.Error(e);
                }

                throw e;
            }

            for (int i = 0; i < NextNodes.Count; i++)
            {
                Node node = NextNodes[i];

                try
                {
                    node.Build(arr.Length > i ? arr[i] : null);
                }
                catch (Exception e)
                {
                    if (e is not NodeStackException)
                    {
                        node.HighlightError();

                        Log.Error(e);
                    }

                    throw e;
                }
            }
        }

        public virtual void HighlightError()
        {
            AddClass("error");
        }

        public virtual void RemoveHighlights()
        {
            RemoveClass("error");
        }

        public void ConnectWithNode(Node node, int index)
        {
            if (node == this)
            {
                return;
            }

            NodeConnectionWire nodeConnectionWire = NodeConnectionWire.Create();

            NodeConnectionStartPoint startPoint = this.NodeSettings[index].Output.ConnectionPoint as NodeConnectionStartPoint;
            startPoint.ConnectionWire = nodeConnectionWire;
            nodeConnectionWire.StartPoint = startPoint;

            NodeConnectionEndPoint endPoint = node.NodeSettings[ConnectPositions[index]].Input.ConnectionPoint as NodeConnectionEndPoint;
            endPoint.ConnectionWire = nodeConnectionWire;
            nodeConnectionWire.EndPoint = endPoint;
        }

        public virtual Dictionary<string, object> GetJsonData()
        {
            List<Dictionary<string, object>> nextNodesJsonList = new();

            foreach (Node node in NextNodes)
            {
                nextNodesJsonList.Add(node.GetJsonData());
            }

            return new Dictionary<string, object>()
            {
                ["LibraryName"] = LibraryName,
                ["ConnectPositions"] = ConnectPositions,
                ["NextNodes"] = nextNodesJsonList
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            VisualProgrammingWindow.Instance.AddNode(this);

            jsonData.TryGetValue("ConnectPositions", out object connectPosition);

            if (connectPosition != null)
            {
                ConnectPositions = JsonSerializer.Deserialize<List<int>>(((JsonElement) connectPosition).GetRawText());
            }

            jsonData.TryGetValue("NextNodes", out object nextNodes);

            if (nextNodes != null)
            {
                JsonElement nextNodesElement = (JsonElement) nextNodes;
                List<Dictionary<string, object>> nextNodesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextNodesElement.GetRawText());

                for (int i = 0; i < nextNodesList.Count; i++)
                {
                    Node node = GetNodeFromJsonData<Node>(nextNodesList[i]);

                    if (node == null)
                    {
                        continue;
                    }

                    NextNodes.Add(node);
                    ConnectWithNode(node, i);
                }
            }
        }

        public static T GetNodeFromJsonData<T>(Dictionary<string, object> jsonData) where T : Node
        {
            jsonData.TryGetValue("LibraryName", out object libraryName);

            if (libraryName == null)
            {
                return null;
            }

            Type type = Utils.GetTypeByLibraryName<T>(libraryName.ToString());

            if (type == null)
            {
                return null;
            }

            T node = Utils.GetObjectByType<T>(type);

            if (node == null)
            {
                return null;
            }

            node.LoadFromJsonData(jsonData);
            node.RemoveHighlights();

            return node;
        }
    }
}
