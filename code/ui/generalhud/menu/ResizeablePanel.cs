using System.Numerics;
using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class ResizeablePanel : Panel
    {
        public bool IsDragging { get; private set; } = false;

        private Vector2 _draggingMouseStartPosition;
        private Vector2 _sizeBeforeDraggingStarted;

        public ResizeablePanel() : base()
        {
            StyleSheet.Load("/ui/generalhud/menu/ResizeablePanel.scss");
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            if (IsVisible)
            {
                _draggingMouseStartPosition = MousePosition;
                _sizeBeforeDraggingStarted = new Vector2(
                    ComputedStyle.Width.Value.GetPixels(Screen.Width),
                    ComputedStyle.Height.Value.GetPixels(Screen.Height)
                );

                IsDragging = true;
            }
        }

        protected override void OnMouseUp(MousePanelEvent e)
        {
            base.OnMouseUp(e);

            IsDragging = false;
        }

        protected override void OnMouseMove(MousePanelEvent e)
        {
            base.OnMouseMove(e);

            if (IsDragging)
            {
                float screenWidth = Screen.Width;
                float screenHeight = Screen.Height;

                Vector2 delta = new Vector2(
                    (MousePosition.x - _draggingMouseStartPosition.x),
                    (MousePosition.y - _draggingMouseStartPosition.y)
                );

                Vector2 position = new Vector2(
                    ComputedStyle.Left.Value.GetPixels(screenWidth),
                    ComputedStyle.Top.Value.GetPixels(screenHeight)
                );

                Vector2 size = new Vector2(
                    ComputedStyle.Width.Value.GetPixels(screenWidth),
                    ComputedStyle.Height.Value.GetPixels(screenHeight)
                );

                Style.Width = Length.Pixels(_sizeBeforeDraggingStarted.x + delta.x);
                Style.Height = Length.Pixels(_sizeBeforeDraggingStarted.y + delta.y);

                Style.Dirty();
            }
        }
    }
}
