using System;
using System.Numerics;

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

            float left = position.x;
            float top = position.y;

            Matrix? matrix = Parent.GlobalMatrix;

            if (matrix != null)
            {
                Matrix4x4 matrix4X4 = matrix.Value.Numerics;

                position.x = position.x - matrix4X4.M41;
                position.y = position.y - matrix4X4.M42;
            }

            if (position.x < 0)
            {
                left = 0f;

                if (matrix != null)
                {
                    left += matrix.Value.Numerics.M41;
                }
            }
            else if (position.x + parentWidth > screenWidth)
            {
                left = (screenWidth - parentWidth);

                if (matrix != null)
                {
                    left += matrix.Value.Numerics.M41;
                }
            }

            if (position.y < 0f)
            {
                top = 0f;

                if (matrix != null)
                {
                    top += matrix.Value.Numerics.M42;
                }
            }
            else if (position.y + parentHeight > screenHeight)
            {
                top = screenHeight - parentHeight;

                if (matrix != null)
                {
                    top += matrix.Value.Numerics.M42;
                }
            }

            Parent.Style.Left = Length.Pixels(left);
            Parent.Style.Top = Length.Pixels(top);

            Parent.Style.Dirty();
        }
    }
}
