using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SpawnableAttribute : Attribute
    {
        public string Categorie = "General";

        public SpawnableAttribute(string categorie = null) : base()
        {
            Categorie = categorie ?? Categorie;
        }
    }

    public abstract class Node : Modal
    {
        public static List<Node> NodeList = new();

        public string Id { get; set; }
        public string LibraryName { get; set; }
        public string[] ConnectionOutputIds { get; set; } = Array.Empty<string>();
        public string[] ConnectionInputIds { get; set; } = Array.Empty<string>();

        public List<NodeSetting> NodeSettings = new();
        public StackNode StackNode;

        public Node(StackNode stackNode) : base()
        {
            StackNode = stackNode;
            LibraryName = Utils.GetLibraryName(GetType());
            StackNode.NodeReference = LibraryName;

            Header.DragHeader.IsFreeDraggable = true;
            Header.DragHeader.IsLocked = false;

            StyleSheet.Load("/ui/visual-programming/nodes/node/Node.scss");

            AddClass("node");
            AddClass("box-shadow");

            Id = Guid.NewGuid().ToString();

            NodeList.Add(this);
        }

        public static NodeAttribute GetAttribute<T>() where T : Node => Library.GetAttribute(typeof(T)) as NodeAttribute;

        public NodeAttribute GetAttribute() => Library.GetAttribute(GetType()) as NodeAttribute;

        public T AddSetting<T>() where T : NodeSetting, new()
        {
            T nodeSetting = new();
            nodeSetting.Node = this;

            Content.AddChild(nodeSetting);
            NodeSettings.Add(nodeSetting);

            if (nodeSetting.Input.Enabled)
            {
                ConnectionInputIds = new string[ConnectionInputIds.Length + 1];
            }

            if (nodeSetting.Output.Enabled)
            {
                ConnectionOutputIds = new string[ConnectionOutputIds.Length + 1];
            }

            return nodeSetting;
        }

        public override void Delete(bool immediate = false)
        {
            NodeList.Remove(this);

            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                nodeSetting.Input.ConnectionPoint.ConnectionWire?.Delete(true);
                nodeSetting.Output.ConnectionPoint.ConnectionWire?.Delete(true);
            }

            ConnectionInputIds = Array.Empty<string>();
            ConnectionOutputIds = Array.Empty<string>();

            base.Delete(immediate);
        }

        protected override void OnRightClick(Sandbox.UI.MousePanelEvent e)
        {
            Window.Instance?.RemoveNode(this);

            Delete(true);

            base.OnRightClick(e);
        }

        private static Node GetConnectedNode(NodeConnectionPoint connectionPoint)
        {
            NodeConnectionWire connectionWire = connectionPoint.ConnectionWire;

            if (connectionWire == null)
            {
                return null;
            }

            NodeConnectionPoint connectedPoint;

            if (connectionPoint is NodeConnectionStartPoint)
            {
                connectedPoint = connectionWire.EndPoint;
            }
            else
            {
                connectedPoint = connectionWire.StartPoint;
            }

            if (connectedPoint == null)
            {
                return null;
            }

            return connectedPoint.Node;
        }

        public bool HasInput()
        {
            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                if (nodeSetting.Input.ConnectionPoint.ConnectionWire != null)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual object[] Build(params object[] input)
        {
            ConnectionInputIds = new string[ConnectionInputIds.Length];
            ConnectionOutputIds = new string[ConnectionOutputIds.Length];

            StackNode.ConnectionInputIds = new string[ConnectionInputIds.Length];
            StackNode.ConnectionOutputIds = new string[ConnectionOutputIds.Length];

            for (int i = 0; i < NodeSettings.Count; i++)
            {
                NodeSetting nodeSetting = NodeSettings[i];

                if (nodeSetting.Input.Enabled)
                {
                    Node connectedNode = GetConnectedNode(nodeSetting.Input.ConnectionPoint);

                    ConnectionInputIds[i] = connectedNode?.Id;
                    StackNode.ConnectionInputIds[i] = connectedNode?.StackNode.Id;
                }

                if (nodeSetting.Output.Enabled)
                {
                    Node connectedNode = GetConnectedNode(nodeSetting.Output.ConnectionPoint);

                    ConnectionOutputIds[i] = connectedNode?.Id;
                    StackNode.ConnectionOutputIds[i] = connectedNode?.StackNode.Id;
                }
            }

            StackNode.SetPos(Box.Rect.left, Box.Rect.top);

            object[] arr;

            try
            {
                arr = StackNode.Test(input);
            }
            catch (Exception e)
            {
                HighlightError();

                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}': ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return null;
                }

                throw;
            }

            return arr;

            // TODO no autocall anymore
        }

        public virtual void HighlightError()
        {
            AddClass("error");
        }

        public virtual void RemoveHighlights()
        {
            RemoveClass("error");
        }

        public void ConnectWithNode(Node node)
        {
            if (node == this)
            {
                return;
            }

            for (int inIndex = 0; inIndex < ConnectionInputIds.Length; inIndex++)
            {
                if (ConnectionInputIds[inIndex] == node.Id)
                {
                    for (int outIndex = 0; outIndex < node.ConnectionOutputIds.Length; outIndex++)
                    {
                        if (node.ConnectionOutputIds[outIndex] == Id)
                        {
                            NodeConnectionWire nodeConnectionWire = NodeConnectionWire.Create();

                            NodeConnectionStartPoint startPoint = node.NodeSettings[outIndex].Output.ConnectionPoint;
                            startPoint.ConnectionWire = nodeConnectionWire;
                            nodeConnectionWire.StartPoint = startPoint;

                            NodeConnectionEndPoint endPoint = NodeSettings[inIndex].Input.ConnectionPoint;
                            endPoint.ConnectionWire = nodeConnectionWire;
                            nodeConnectionWire.EndPoint = endPoint;

                            return;
                        }
                    }

                    break;
                }
            }

            for (int outIndex = 0; outIndex < ConnectionOutputIds.Length; outIndex++)
            {
                if (ConnectionOutputIds[outIndex] == node.Id)
                {
                    for (int inIndex = 0; inIndex < node.ConnectionInputIds.Length; inIndex++)
                    {
                        if (node.ConnectionInputIds[inIndex] == Id)
                        {
                            NodeConnectionWire nodeConnectionWire = NodeConnectionWire.Create();

                            NodeConnectionStartPoint startPoint = NodeSettings[outIndex].Output.ConnectionPoint;
                            startPoint.ConnectionWire = nodeConnectionWire;
                            nodeConnectionWire.StartPoint = startPoint;

                            NodeConnectionEndPoint endPoint = node.NodeSettings[inIndex].Input.ConnectionPoint;
                            endPoint.ConnectionWire = nodeConnectionWire;
                            nodeConnectionWire.EndPoint = endPoint;

                            return;
                        }
                    }

                    break;
                }
            }
        }

        public virtual Dictionary<string, object> GetJsonData()
        {
            return new Dictionary<string, object>()
            {
                ["Id"] = Id,
                ["LibraryName"] = LibraryName,
                ["ConnectionInputIds"] = ConnectionInputIds,
                ["ConnectionOutputIds"] = ConnectionOutputIds,
                ["Pos"] = new Vector2(Box.Rect.left, Box.Rect.top),
            };
        }

        public virtual void LoadFromJsonData(Dictionary<string, object> jsonData)
        {
            jsonData.TryGetValue("Id", out object id);

            if (id != null)
            {
                Id = id.ToString();
            }

            jsonData.TryGetValue("ConnectionInputIds", out object connectionInputIds);

            if (connectionInputIds != null)
            {
                ConnectionInputIds = JsonSerializer.Deserialize<string[]>(((JsonElement) connectionInputIds).GetRawText());
            }

            jsonData.TryGetValue("ConnectionOutputIds", out object connectionOutputIds);

            if (connectionOutputIds != null)
            {
                ConnectionOutputIds = JsonSerializer.Deserialize<string[]>(((JsonElement) connectionOutputIds).GetRawText());
            }

            jsonData.TryGetValue("Pos", out object pos);

            if (pos != null)
            {
                Vector2 vector2 = (Vector2) pos;

                Style.Left = Sandbox.UI.Length.Pixels(vector2.x);
                Style.Top = Sandbox.UI.Length.Pixels(vector2.y);
            }

            // TODO connect nodes
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

            Window.Instance.AddNode(node);

            node.LoadFromJsonData(jsonData);
            node.RemoveHighlights();

            return node;
        }
    }
}
