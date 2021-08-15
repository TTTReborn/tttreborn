namespace TTTReborn.UI
{
    public partial class Switch : Sandbox.UI.Switch
    {
        public Switch() : base()
        {
            StyleSheet.Load("/ui/components/switch/Switch.scss");
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class SwitchConstructor
    {
        public static Switch Switch(this PanelCreator self, string className = null, bool enabled = false)
        {
            Switch sw = self.panel.AddChild<Switch>();

            if (!string.IsNullOrEmpty(className))
            {
                sw.AddClass(className);
            }

            sw.Checked = enabled;

            return sw;
        }
    }
}
