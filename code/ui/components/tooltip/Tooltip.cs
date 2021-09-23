using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Tooltip : Label
    {
        public static List<Tooltip> Tooltips = new();

        public readonly Sandbox.UI.Panel RelatedPanel;

        public float RequiredHoveringTime { get; set; } = 0.5f;

        private TimeSince _timeSinceMouseStopped = 0f;

        public Tooltip(Sandbox.UI.Panel relatedPanel) : base()
        {
            RelatedPanel = relatedPanel;
            Parent = Hud.Current.RootPanel;

            StyleSheet.Load("/ui/components/tooltip/Tooltip.scss");

            AddClass("hide");

            Rect rect = RelatedPanel.Box.Rect;

            Style.Left = Length.Pixels(rect.left);
            Style.Top = Length.Pixels(rect.top);
            Style.Width = Length.Pixels(rect.width);
            Style.Dirty();

            Tooltips.Add(this);
        }

        public override void OnDeleted()
        {
            Tooltips.Remove(this);
        }

        public override void Tick()
        {
            base.Tick();

            if (Mouse.Delta != Vector2.Zero)
            {
                _timeSinceMouseStopped = 0f;
            }

            SetClass("hide", _timeSinceMouseStopped < RequiredHoveringTime);
        }
    }
}

namespace Sandbox.UI
{
    using TTTReborn.UI;

    public static class TooltipConstructor
    {
        public static void AddTooltip(this Panel self, string text = "", string className = null)
        {
            self.AddEventListener("onmouseover", (panelEvent) =>
            {
                if (Mouse.Delta == Vector2.Zero)
                {
                    return;
                }

                Tooltip tooltip = new(self);
                tooltip.SetText(text);

                if (!string.IsNullOrEmpty(className))
                {
                    tooltip.AddClass(className);
                }
            });

            self.AddEventListener("onmouseout", (panelEvent) =>
            {
                if (Mouse.Delta == Vector2.Zero)
                {
                    return;
                }

                foreach (Tooltip tooltip in Tooltip.Tooltips)
                {
                    if (tooltip.RelatedPanel == self)
                    {
                        tooltip.Delete();
                    }
                }
            });
        }
    }
}
