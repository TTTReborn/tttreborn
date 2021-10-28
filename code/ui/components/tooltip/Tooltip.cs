using System;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Tooltip : TranslationLabel
    {
        public static Tooltip Instance;

        public float RequiredHoveringTime { get; set; } = 0.5f;

        public readonly Action<Tooltip> OnCreate;
        public readonly Action<Tooltip> OnDelete;
        public readonly Action<Tooltip> OnTick;
        public readonly Sandbox.UI.Panel RelatedPanel;

        private TimeSince _timeSinceMouseStopped = 0f;

        public Tooltip(Sandbox.UI.Panel relatedPanel, Action<Tooltip> onCreate = null, Action<Tooltip> onDelete = null, Action<Tooltip> onTick = null) : base()
        {
            RelatedPanel = relatedPanel;
            Parent = Hud.Current.RootPanel;
            OnCreate = onCreate;
            OnDelete = onDelete;
            OnTick = onTick;

            StyleSheet.Load("/ui/components/tooltip/Tooltip.scss");

            AddClass("hide");

            Rect rect = RelatedPanel.Box.Rect;

            Style.Left = Length.Pixels(rect.left);
            Style.Top = Length.Pixels(rect.top);
            Style.MinWidth = Length.Pixels(200f);
            Style.MaxWidth = Length.Pixels(Math.Max(rect.width, 200f));
            Style.Dirty();

            Instance = this;
            IsTryTranslation = true;
        }

        public override void OnDeleted()
        {
            Instance = null;
        }

        public override void Tick()
        {
            base.Tick();

            if (Mouse.Delta != Vector2.Zero)
            {
                _timeSinceMouseStopped = 0f;
            }

            SetClass("hide", _timeSinceMouseStopped < RequiredHoveringTime);

            OnTick?.Invoke(this);
        }
    }
}

namespace Sandbox.UI.Construct
{
    using TTTReborn.UI;

    public static class TooltipConstructor
    {
        public static void AddTooltip(this Sandbox.UI.Panel self, string text = "", string className = null, Action<Tooltip> onCreate = null, Action<Tooltip> onDelete = null, Action<Tooltip> onTick = null, params object[] translationData)
        {
            self.AddEventListener("onmouseover", (panelEvent) =>
            {
                if (Mouse.Delta == Vector2.Zero)
                {
                    return;
                }

                CreateTooltip(self, text, className, onCreate, onDelete, onTick, translationData);
            });

            self.AddEventListener("onmouseout", (panelEvent) =>
            {
                if (Mouse.Delta == Vector2.Zero)
                {
                    return;
                }

                DeleteTooltip();
            });

            self.AddEventListener("onclick", (panelEvent) =>
            {
                CreateTooltip(self, text, className, onCreate, onDelete, onTick, translationData);
            });
        }

        private static Tooltip CreateTooltip(Sandbox.UI.Panel panel, string text = "", string className = null, Action<Tooltip> onCreate = null, Action<Tooltip> onDelete = null, Action<Tooltip> onTick = null, params object[] translationData)
        {
            DeleteTooltip();

            Tooltip tooltip = new(panel, onCreate, onDelete, onTick);
            tooltip.SetTranslation(text, translationData);

            if (!string.IsNullOrEmpty(className))
            {
                tooltip.AddClass(className);
            }

            onCreate?.Invoke(tooltip);

            return tooltip;
        }

        private static void DeleteTooltip()
        {
            Tooltip.Instance?.OnDelete?.Invoke(Tooltip.Instance);
            Tooltip.Instance?.Delete(true);
        }
    }
}
