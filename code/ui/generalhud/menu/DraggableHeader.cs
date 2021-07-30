using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class DraggableHeader : Panel
    {
        public bool IsDragging { get; private set; } = false;

        private Vector2 _draggingMouseStartPosition;

        public DraggableHeader(Panel parent)
        {
            Parent = parent;

            StyleSheet.Load("/ui/generalhud/menu/DraggableHeader.scss");
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            if (IsVisible)
            {
                _draggingMouseStartPosition = MousePosition;
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

                Vector2 position = new Vector2(
                    (MousePosition.x - _draggingMouseStartPosition.x) + Parent.ComputedStyle.Left.Value.GetPixels(screenWidth),
                    (MousePosition.y - _draggingMouseStartPosition.y) + Parent.ComputedStyle.Top.Value.GetPixels(screenHeight)
                );

                float parentWidth = Parent.ComputedStyle.Width.Value.GetPixels(screenWidth);
                float parentHeight = Parent.ComputedStyle.Height.Value.GetPixels(screenHeight);

                if (position.x < 0f)
                {
                    Parent.Style.Left = Length.Pixels(0f);
                }
                else if (position.x + parentWidth > screenWidth)
                {
                    Parent.Style.Left = Length.Pixels(screenWidth - parentWidth);
                }
                else
                {
                    Parent.Style.Left = Length.Pixels(position.x);
                }

                if (position.y < 0f)
                {
                    Parent.Style.Top = Length.Pixels(0f);
                }
                else if (position.y + parentHeight > screenHeight)
                {
                    Parent.Style.Top = Length.Pixels(screenHeight - parentHeight);
                }
                else
                {
                    Parent.Style.Top = Length.Pixels(position.y);
                }

                Parent.Style.Dirty();
            }
        }

        private bool IsInScreen(Vector2 position)
        {
            if (position.x < 0f || position.x > Screen.Width)
            {
                return false;
            }
            if (position.y < 0f || position.y > Screen.Height)
            {
                return false;
            }

            return true;
        }
    }
}
