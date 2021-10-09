using System;
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
            Style.MinWidth = Length.Pixels(200f);
            Style.MaxWidth = Length.Pixels(Math.Max(rect.width, 200f));
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

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TooltipConstructor
    {
        public static void AddTooltip(this Sandbox.UI.Panel self, string text = "", string className = null, Action<Tooltip> onCreate = null, Action<Tooltip> onDelete = null)
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

                onCreate?.Invoke(tooltip);
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

                    onDelete?.Invoke(tooltip);
                }
            });
        }
    }
}
