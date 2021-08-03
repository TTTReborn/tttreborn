using System.Numerics;

using Sandbox;
using Sandbox.UI;

// TODO use M4x4 transform

namespace TTTReborn.UI
{
    public partial class DragDrop : Panel
    {
        public bool IsDragging { get; private set; } = false;

        public bool IsFreeDraggable { get; set; } = false;

        public string DragDropGroupName { get; set; } = "";

        public Panel DragBasePanel
        {
            get => _dragBasePanel ?? this;
            set
            {
                _dragBasePanel = value;
            }
        }
        private Panel _dragBasePanel;

        private Vector2 _draggingMouseStartPosition;
        private Vector2? _draggingStartPosition;

        public DragDrop(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;

            StyleSheet.Load("/ui/components/dragdrop/DragDrop.scss");
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            if (!IsVisible)
            {
                return;
            }

            _draggingMouseStartPosition = MousePosition;
            _draggingStartPosition = null;
            IsDragging = true;
        }

        protected override void OnMouseUp(MousePanelEvent e)
        {
            if (IsDragging)
            {
                if (!IsFreeDraggable)
                {
                    DragDrop TargetDragDrop = null; // TODO get the right hovering panel

                    if (TargetDragDrop != null && DragDropGroupName == TargetDragDrop.DragDropGroupName)
                    {
                        OnDragPanelFinished(TargetDragDrop);
                    }
                    else
                    {
                        OnDragPanelFailed(TargetDragDrop);
                    }
                }

                IsDragging = false;
            }
        }

        protected override void OnMouseMove(MousePanelEvent e)
        {
            if (!IsDragging)
            {
                return;
            }

            if (_draggingStartPosition == null)
            {
                _draggingStartPosition = new Vector2(DragBasePanel.Box.Rect.left, DragBasePanel.Box.Rect.top);
            }

            Vector2 position = new Vector2(
                (MousePosition.x - _draggingMouseStartPosition.x) + DragBasePanel.Box.Rect.left,
                (MousePosition.y - _draggingMouseStartPosition.y) + DragBasePanel.Box.Rect.top
            );

            float screenWidth = Screen.Width;
            float screenHeight = Screen.Height;

            float parentWidth = DragBasePanel.Box.Rect.width;
            float parentHeight = DragBasePanel.Box.Rect.height;

            float left = position.x;
            float top = position.y;

            Matrix? matrix = DragBasePanel.GlobalMatrix;

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

            OnDragPanel(left, top);
        }

        public virtual void OnDragPanel(float left, float top)
        {
            DragBasePanel.Style.Left = Length.Pixels(left);
            DragBasePanel.Style.Top = Length.Pixels(top);

            DragBasePanel.Style.Dirty();
        }

        public virtual void OnDragPanelFinished(DragDrop targetDragDrop)
        {
            targetDragDrop.AddChild(this);

            // TODO add to the right position

            DragBasePanel.Style.Left = null;
            DragBasePanel.Style.Top = null;

            DragBasePanel.Style.Dirty();
        }

        public virtual void OnDragPanelFailed(DragDrop targetDragDrop)
        {
            DragBasePanel.Style.Left = Length.Pixels(_draggingStartPosition?.x ?? 0f);
            DragBasePanel.Style.Top = Length.Pixels(_draggingStartPosition?.y ?? 0f);

            DragBasePanel.Style.Dirty();
        }
    }
}
