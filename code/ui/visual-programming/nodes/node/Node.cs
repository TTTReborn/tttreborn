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
        public static readonly List<Node> NodeList = new();

        public string Id { get; set; }
        public string LibraryName { get; set; }
        public string[] ConnectionOutputIds { get; set; }
        public string[] ConnectionInputIds { get; set; }
        public object[] InputData { get; set; }

        public List<NodeSetting> NodeSettings;
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
            StackNode.Id = Id;

            ConnectionOutputIds = Array.Empty<string>();
            ConnectionInputIds = Array.Empty<string>();
            InputData = Array.Empty<object>();

            NodeSettings = new();

            NodeList.Add(this);
        }

        public static Node GetById(string id)
        {
            foreach (Node node in NodeList)
            {
                if (node.Id == id)
                {
                    return node;
                }
            }

            return null;
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
                string[] oldArr = ConnectionInputIds;

                ConnectionInputIds = new string[ConnectionInputIds.Length + 1];

                for (int i = 0; i < oldArr.Length; i++)
                {
                    ConnectionInputIds[i] = oldArr[i];
                }
            }

            if (nodeSetting.Output.Enabled)
            {
                string[] oldArr = ConnectionOutputIds;

                ConnectionOutputIds = new string[ConnectionOutputIds.Length + 1];

                for (int i = 0; i < oldArr.Length; i++)
                {
                    ConnectionOutputIds[i] = oldArr[i];
                }
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

            Reset();

            StackNode.Delete();

            base.Delete(immediate);
        }

        public virtual void Reset()
        {
            ResetData();
            RemoveHighlights();

            StackNode.Reset();
        }

        public virtual void ResetData()
        {
            ConnectionInputIds = Array.Empty<string>();
            ConnectionOutputIds = Array.Empty<string>();
            InputData = Array.Empty<object>();
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

        public bool HasInputsFilled()
        {
            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                if (nodeSetting.Input.Enabled && nodeSetting.Input.ConnectionPoint.ConnectionWire == null)
                {
                    return false;
                }
            }

            return true;
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

        public bool HasInputEnabled()
        {
            foreach (NodeSetting nodeSetting in NodeSettings)
            {
                if (nodeSetting.Input.Enabled)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void Prepare()
        {
            if (StackNode.ConnectionInputIds.Length == 0 || StackNode.ConnectionOutputIds.Length == 0)
            {
                int inputCount = 0, outputCount = 0;

                for (int i = 0; i < NodeSettings.Count; i++)
                {
                    NodeSetting nodeSetting = NodeSettings[i];

                    if (nodeSetting.Input.Enabled)
                    {
                        inputCount++;
                    }

                    if (nodeSetting.Output.Enabled)
                    {
                        outputCount++;
                    }
                }

                ConnectionInputIds = new string[inputCount];
                StackNode.ConnectionInputIds = new string[inputCount];

                ConnectionOutputIds = new string[outputCount];
                StackNode.ConnectionOutputIds = new string[outputCount];

                int inputConnectionCount = 0;

                for (int i = 0; i < NodeSettings.Count; i++)
                {
                    NodeSetting nodeSetting = NodeSettings[i];

                    if (nodeSetting.Input.Enabled)
                    {
                        Node connectedNode = GetConnectedNode(nodeSetting.Input.ConnectionPoint);

                        ConnectionInputIds[i] = connectedNode?.Id;
                        StackNode.ConnectionInputIds[i] = connectedNode?.StackNode.Id;

                        if (connectedNode != null)
                        {
                            inputConnectionCount++;
                        }
                    }

                    if (nodeSetting.Output.Enabled)
                    {
                        Node connectedNode = GetConnectedNode(nodeSetting.Output.ConnectionPoint);

                        ConnectionOutputIds[i] = connectedNode?.Id;
                        StackNode.ConnectionOutputIds[i] = connectedNode?.StackNode.Id;
                    }
                }

                InputData = new object[inputConnectionCount];

                StackNode.SetPos(Box.Rect.left, Box.Rect.top);
            }
        }

        public virtual bool Build(int inputIndex, object input = null)
        {
            object[] buildInput;

            if (InputData.Length > 0)
            {
                InputData[inputIndex] = input;

                for (int i = 0; i < InputData.Length; i++)
                {
                    if (InputData[i] == null)
                    {
                        return true;
                    }
                }

                buildInput = InputData;
            }
            else
            {
                buildInput = new object[]
                {
                    input
                };
            }

            bool successful = true;

            try
            {
                object[] array = StackNode.Test(buildInput);

                for (int o = 0; o < ConnectionOutputIds.Length; o++)
                {
                    string id = ConnectionOutputIds[o];

                    if (id == null)
                    {
                        continue;
                    }

                    Node idNode = GetById(id);

                    if (idNode == null)
                    {
                        Log.Warning($"Error in building NodeStack with node {Id} ('{LibraryName}')");

                        return false;
                    }

                    int inputCount = 0;

                    for (int i = 0; i < idNode.NodeSettings.Count; i++)
                    {
                        NodeSetting nodeSetting = idNode.NodeSettings[i];

                        if (nodeSetting.Input.Enabled)
                        {
                            if (GetConnectedNode(nodeSetting.Input.ConnectionPoint) == this)
                            {
                                if (!idNode.Build(inputCount, array[o]))
                                {
                                    successful = false;
                                }

                                break;
                            }

                            inputCount++;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HighlightError();

                if (e is NodeStackException)
                {
                    Log.Warning($"Error in node '{GetType()}': ({e.Source}): {e.Message}\n{e.StackTrace}");

                    return false;
                }

                throw;
            }

            return successful;
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

            for (int outIndex = 0; outIndex < ConnectionOutputIds.Length; outIndex++)
            {
                if (ConnectionOutputIds[outIndex] == node.Id)
                {
                    for (int inIndex = 0; inIndex < node.ConnectionInputIds.Length; inIndex++)
                    {
                        if (node.ConnectionInputIds[inIndex] == Id)
                        {
                            NodeConnectionWire nodeConnectionWire = NodeConnectionWire.Create();

                            int outCount = 0;

                            foreach (NodeSetting nodeSetting in NodeSettings)
                            {
                                if (nodeSetting.Output.Enabled)
                                {
                                    if (outCount == outIndex)
                                    {
                                        NodeConnectionStartPoint startPoint = nodeSetting.Output.ConnectionPoint;
                                        startPoint.ConnectionWire = nodeConnectionWire;
                                        nodeConnectionWire.StartPoint = startPoint;
                                    }

                                    outCount++;
                                }
                            }

                            int inCount = 0;

                            foreach (NodeSetting nodeSetting in node.NodeSettings)
                            {
                                if (nodeSetting.Input.Enabled)
                                {
                                    if (inCount == inIndex)
                                    {
                                        NodeConnectionEndPoint endPoint = nodeSetting.Input.ConnectionPoint;
                                        endPoint.ConnectionWire = nodeConnectionWire;
                                        nodeConnectionWire.EndPoint = endPoint;
                                    }

                                    inCount++;
                                }
                            }

                            return;
                        }
                    }

                    break;
                }
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

                            int outCount = 0;

                            foreach (NodeSetting nodeSetting in node.NodeSettings)
                            {
                                if (nodeSetting.Output.Enabled)
                                {
                                    if (outCount == outIndex)
                                    {
                                        NodeConnectionStartPoint startPoint = nodeSetting.Output.ConnectionPoint;
                                        startPoint.ConnectionWire = nodeConnectionWire;
                                        nodeConnectionWire.StartPoint = startPoint;
                                    }

                                    outCount++;
                                }
                            }

                            int inCount = 0;

                            foreach (NodeSetting nodeSetting in NodeSettings)
                            {
                                if (nodeSetting.Input.Enabled)
                                {
                                    if (inCount == inIndex)
                                    {
                                        NodeConnectionEndPoint endPoint = nodeSetting.Input.ConnectionPoint;
                                        endPoint.ConnectionWire = nodeConnectionWire;
                                        nodeConnectionWire.EndPoint = endPoint;
                                    }

                                    inCount++;
                                }
                            }

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
                Id = ((JsonElement) id).ToString();
                StackNode.Id = Id;
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
                Vector2 vector2 = JsonSerializer.Deserialize<Vector2>(((JsonElement) pos).GetRawText());
                float scale = Hud.Current.GeneralHudPanel.ScaleToScreen;

                Style.Left = Sandbox.UI.Length.Pixels(vector2.x / scale);
                Style.Top = Sandbox.UI.Length.Pixels(vector2.y / scale);
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

            Window.Instance.AddNode(node);

            node.LoadFromJsonData(jsonData);
            node.RemoveHighlights();

            return node;
        }
    }
}
