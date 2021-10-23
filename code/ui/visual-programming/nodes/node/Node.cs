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

        private Node GetConnectedNode(NodeConnectionPoint connectionPoint)
        {
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

            return connectedPoint.Node;
        }

        public virtual void Build(params object[] input)
        {
            NextNodes.Clear();

            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                if (nodeSetting.Output == null)
                {
                    continue;
                }

                foreach (NodeConnectionPoint nodeConnectionPoint in nodeSetting.Output.ConnectionPoints)
                {
                    Node connectedNode = GetConnectedNode(nodeConnectionPoint);

                    if (connectedNode == null)
                    {
                        continue;
                    }

                    NextNodes.Add(connectedNode);
                    StackNode.NextNodes.Add(connectedNode.StackNode);
                }
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
                NextNodes[i].Build(arr.Length > i ? arr[i] : null);
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
                ["NextNodes"] = nextNodesJsonList
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            VisualProgrammingWindow.Instance.AddNode(this);

            jsonData.TryGetValue("NextNodes", out object nextNodes);

            if (nextNodes == null)
            {
                return;
            }

            JsonElement nextNodesElement = (JsonElement) nextNodes;
            List<Dictionary<string, object>> nextNodesList = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(nextNodesElement.GetRawText());

            foreach (Dictionary<string, object> nodeJsonData in nextNodesList)
            {
                Node node = GetNodeFromJsonData(nodeJsonData);

                if (node == null)
                {
                    continue;
                }

                NextNodes.Add(node);
            }
        }

        public static Node GetNodeFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("LibraryName", out object libraryName);

            if (libraryName == null)
            {
                return null;
            }

            Log.Error($"Got node {libraryName.ToString()}");

            Type type = Utils.GetTypeByLibraryName<Node>(libraryName.ToString());

            jsonData.Remove("LibraryName");

            if (type == null)
            {
                return null;
            }

            Log.Error("Got type");

            Node node = Utils.GetObjectByType<Node>(type);

            if (node == null)
            {
                return null;
            }

            Log.Error("Got node");

            node.LoadFromJsonData(jsonData);

            return node;
        }
    }
}
