using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;

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
        public string LibraryName { get; set; }
        public List<NodeSetting> NodeSettings { get; set; } = new();

        public Node() : base()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            Header.DragHeader.IsFreeDraggable = true;
            Header.DragHeader.IsLocked = false;

            StyleSheet.Load("/ui/visual-programming/nodes/node/Node.scss");

            AddClass("node");
            AddClass("box-shadow");
        }

        public static NodeAttribute GetAttribute<T>() where T : Node
        {
            return Library.GetAttribute(typeof(T)) as NodeAttribute;
        }

        public NodeAttribute GetAttribute()
        {
            return Library.GetAttribute(GetType()) as NodeAttribute;
        }

        public T AddSetting<T>() where T : NodeSetting, new()
        {
            T nodeSetting = new();
            nodeSetting.Node = this;

            Content.AddChild(nodeSetting);
            NodeSettings.Add(nodeSetting);

            return nodeSetting;
        }

        public virtual void Evaluate(params object[] input)
        {
            foreach (NodeSetting setting in NodeSettings)
            {
                if (setting.Output == null)
                {
                    continue;
                }

                foreach (NodeConnectionPoint connectionPoint in setting.Output.ConnectionPoints)
                {
                    NodeConnectionWire connectionWire = connectionPoint.ConnectionWire;

                    if (connectionWire == null)
                    {
                        continue;
                    }

                    NodeConnectionPoint connectedPoint = connectionWire.EndPoint;

                    if (connectedPoint == null)
                    {
                        continue;
                    }

                    Node connectedNode = connectedPoint.Node;

                    if (connectedNode == null)
                    {
                        continue;
                    }

                    connectedNode.Evaluate(input);
                }
            }
        }
    }
}
