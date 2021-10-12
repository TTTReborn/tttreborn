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

        public NodeConnectionPanel Input { get; set; }
        public NodeConnectionPanel Output { get; set; }
        public PanelContent Content { get; set; }

        public NodeSetting() : base()
        {
            LibraryName = GetAttribute().Name;

            Input = new(this);
            Content = new(this);
            Output = new(this);

            AddClass("nodesetting");
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
