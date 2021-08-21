using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Tooltip : Label
    {
        public float RequiredHoveringTime { get; set; } = 0.5f;

        private TimeSince _timeSinceMouseStopped = 0f;
        private Vector2 _lastMousePosition = Vector2.Zero;
        private bool _hovering = false;

        public Tooltip(Panel parent = null) : base()
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/tooltip/Tooltip.scss");

            Parent.AddEventListener("onmouseover", (panelEvent) =>
            {
                _hovering = true;
            });

            Parent.AddEventListener("onmouseout", (panelEvent) =>
            {
                _hovering = false;
            });

            AddClass("hide");
        }

        public override void Tick()
        {
            base.Tick();

            if (_lastMousePosition != Mouse.Position)
            {
                _lastMousePosition = Mouse.Position;

                _timeSinceMouseStopped = 0f;
            }

            SetClass("hide", !_hovering || _timeSinceMouseStopped < RequiredHoveringTime);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TooltipConstructor
    {
        public static Tooltip Tooltip(this PanelCreator self, string text = "", string className = null)
        {
            Tooltip tooltip = new Tooltip(self.panel);
            tooltip.SetText(text);

            if (!string.IsNullOrEmpty(className))
            {
                tooltip.AddClass(className);
            }

            return tooltip;
        }
    }
}
