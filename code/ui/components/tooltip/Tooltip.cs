using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Tooltip : Label
    {
        public float RequiredHoveringTime { get; set; } = 0.5f;
        public Panel RelatedPanel { get; private set; }

        private TimeSince _timeSinceMouseStopped = 0f;
        private Vector2 _lastMousePosition = Vector2.Zero;
        private bool _hovering = false;

        public Tooltip(Panel relatedPanel = null) : base()
        {
            RelatedPanel = relatedPanel ?? Parent;

            Hud.Current.RootPanel.AddChild(this);

            StyleSheet.Load("/ui/components/tooltip/Tooltip.scss");

            RelatedPanel.AddEventListener("onmouseover", (panelEvent) =>
            {
                _hovering = true;
            });

            RelatedPanel.AddEventListener("onmouseout", (panelEvent) =>
            {
                _hovering = false;
            });

            AddClass("hide");
        }

        public override void Tick()
        {
            base.Tick();

            bool hide = !_hovering || _timeSinceMouseStopped < RequiredHoveringTime;

            if (_lastMousePosition != Mouse.Position)
            {
                _lastMousePosition = Mouse.Position;
                _timeSinceMouseStopped = 0f;
            }
            else if (HasClass("hide") && !hide)
            {
                Rect rect = RelatedPanel.Box.Rect;

                Style.Left = Length.Pixels(rect.left);
                Style.Top = Length.Pixels(rect.top);
                Style.Width = Length.Pixels(rect.width);
                Style.Dirty();
            }

            SetClass("hide", hide);
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
