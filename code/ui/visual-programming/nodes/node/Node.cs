using System;
using System.Collections.Generic;

using Sandbox;

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

        public Node(Sandbox.UI.Panel parent = null) : base(parent)
        {
            LibraryName = GetAttribute().Name;

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
            T nodeSetting = new T();

            Content.AddChild(nodeSetting);
            NodeSettings.Add(nodeSetting);

            return nodeSetting;
        }
    }
}
