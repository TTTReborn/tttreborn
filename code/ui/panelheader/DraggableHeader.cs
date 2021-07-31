using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class DraggableHeader : PanelHeader
    {
        public bool IsDragging { get; private set; } = false;

        private Vector2 _draggingMouseStartPosition;

        public DraggableHeader(Panel parent) : base()
        {
            Parent = parent;

            StyleSheet.Load("/ui/panelheader/DraggableHeader.scss");
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            if (!IsVisible)
            {
                return;
            }

            _draggingMouseStartPosition = MousePosition;
            IsDragging = true;
        }

        protected override void OnMouseUp(MousePanelEvent e)
        {
            IsDragging = false;
        }

        protected override void OnMouseMove(MousePanelEvent e)
        {
            if (!IsDragging)
            {
                return;
            }

            Vector2 position = new Vector2(
                (MousePosition.x - _draggingMouseStartPosition.x) + Parent.Box.Rect.left,
                (MousePosition.y - _draggingMouseStartPosition.y) + Parent.Box.Rect.top
            );

            float screenWidth = Screen.Width;
            float screenHeight = Screen.Height;

            float parentWidth = Parent.Box.Rect.width;
            float parentHeight = Parent.Box.Rect.height;

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
