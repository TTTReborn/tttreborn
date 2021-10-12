using System;

namespace TTTReborn.UI
{
    public partial class Switch : Sandbox.UI.Switch
    {
        public bool Disabled
        {
            get => _disabled;
            set
            {
                _disabled = value;

                SetClass("disable", _disabled);
            }
        }
        private bool _disabled = false;

        public Func<Sandbox.UI.MousePanelEvent, bool> OnCheck;

        public Switch(Sandbox.UI.Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/switch/Switch.scss");
        }

        protected override void OnClick(Sandbox.UI.MousePanelEvent e)
        {
            if (!Disabled && (OnCheck == null || OnCheck.Invoke(e)))
            {
                base.OnClick(e);
            }

            e.StopPropagation();
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
            Switch sw = new(self.panel);

            if (!string.IsNullOrEmpty(className))
            {
                sw.AddClass(className);
            }

            sw.Checked = enabled;

            return sw;
        }
    }
}
