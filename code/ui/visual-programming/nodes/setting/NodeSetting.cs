using System;

using Sandbox;

namespace TTTReborn.UI.VisualProgramming
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NodeSettingAttribute : LibraryAttribute
    {
        public NodeSettingAttribute(string name) : base(name)
        {

        }
    }

    public abstract class NodeSetting : Panel
    {
        public string LibraryName { get; set; }

        public Node Node
        {
            get => _node;
            set
            {
                _node = value;

                if (Input != null)
                {
                    Input.Node = _node;
                }

                if (Output != null)
                {
                    Output.Node = _node;
                }
            }
        }
        private Node _node;

        public NodeConnectionPanel Input { get; set; }
        public NodeConnectionPanel Output { get; set; }
        public PanelContent Content { get; set; }

        public NodeSetting() : base()
        {
            LibraryName = GetAttribute().Name;

            Input = AddConnectionPanel();
            Input.AddConnectionPoint<NodeConnectionEndPoint>();

            Content = new(this);

            Output = AddConnectionPanel();
            Output.AddConnectionPoint<NodeConnectionStartPoint>();

            AddClass("nodesetting");
        }

        public NodeConnectionPanel AddConnectionPanel()
        {
            NodeConnectionPanel nodeConnectionPanel = new(this);
            nodeConnectionPanel.Node = Node;

            return nodeConnectionPanel;
        }

        public static NodeSettingAttribute GetAttribute<T>() where T : NodeSetting
        {
            return Library.GetAttribute(typeof(T)) as NodeSettingAttribute;
        }

        public NodeSettingAttribute GetAttribute()
        {
            return Library.GetAttribute(GetType()) as NodeSettingAttribute;
        }

        public void ToggleInput(bool toggle)
        {
            Input.Enabled = toggle;
        }

        public void ToggleOutput(bool toggle)
        {
            Output.Enabled = toggle;
        }
    }
}
