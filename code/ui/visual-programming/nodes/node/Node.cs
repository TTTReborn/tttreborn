using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.VisualProgramming;

namespace TTTReborn.UI.VisualProgramming
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeAttribute : LibraryAttribute
    {
        public NodeAttribute(string name) : base(name)
        {

        }
    }

    public abstract class Node : Modal
    {
        public StackNode StackNode;
        public string LibraryName { get; set; }
        public List<NodeSetting> NodeSettings { get; set; } = new();

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
            List<Node> Nodes = new();

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

                    Nodes.Add(connectedNode);
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

                    return;
                }

                Log.Error(e);
            }

            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].Build(arr.Length > i ? arr[i] : null);
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
    }
}
