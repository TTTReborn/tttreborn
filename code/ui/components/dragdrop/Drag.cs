using System.Numerics;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Drag : DragDrop
    {
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                _isLocked = value;

                SetClass("locked", _isLocked);
            }
        }
        private bool _isLocked = false;

        public bool IsDragging
        {
            get => _isDragging;
            private set
            {
                _isDragging = value;

                if (value == false)
                {
                    DragBasePanel.Style.Position = _oldPositionMode;

                    _oldPositionMode = null;
                }
                else
                {
                    if (_oldPositionMode == null)
                    {
                        _oldPositionMode = DragBasePanel.Style.Position;
                    }

                    DragBasePanel.Style.Position = PositionMode.Absolute;
                }
            }
        }
        private bool _isDragging = false;

        public bool IsFreeDraggable
        {
            get => _isFreeDraggable;
            set
            {
                _isFreeDraggable = value;

                DragBasePanel.Style.Position = PositionMode.Absolute;
            }
        }
        private bool _isFreeDraggable = false;

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
        private Vector2 _draggingStartPosition;
        private Length? _oldPositionLeft;
        private Length? _oldPositionTop;
        private PositionMode? _oldPositionMode;

        public Drag(Panel parent = null) : base(parent)
        {
            StyleSheet.Load("/ui/components/dragdrop/Drag.scss");

            AddClass("drag");

            IsLocked = false;
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            if (!IsVisible || IsLocked)
            {
                return;
            }

            _draggingMouseStartPosition = Mouse.Position;
            _draggingStartPosition = new Vector2(DragBasePanel.ComputedStyle.Left?.GetPixels(Screen.Width) ?? 0f, DragBasePanel.ComputedStyle.Top?.GetPixels(Screen.Height) ?? 0f);
            _oldPositionLeft = DragBasePanel.Style.Left;
            _oldPositionTop = DragBasePanel.Style.Top;

            // TODO move child to latest child (high zindex)

            IsDragging = true;
        }

        protected override void OnMouseUp(MousePanelEvent e)
        {
            if (IsDragging)
            {
                bool noDropTarget = string.IsNullOrEmpty(DragDropGroupName);

                Drop TargetDrop = noDropTarget ? null : GetDropPanel(); // TODO get the right hovering position

                if (noDropTarget || TargetDrop != null && DragDropGroupName == TargetDrop.DragDropGroupName)
                {
                    OnDragPanelSuccess(TargetDrop);
                }
                else
                {
                    OnDragPanelFailed(TargetDrop);
                }

                OnDragPanelFinished();

                IsDragging = false;
            }
        }

        protected override void OnMouseMove(MousePanelEvent e)
        {
            if (!IsDragging)
            {
                return;
            }

            Vector2 position = new()
            {
                x = Mouse.Position.x - _draggingMouseStartPosition.x + _draggingStartPosition.x,
                y = Mouse.Position.y - _draggingMouseStartPosition.y + _draggingStartPosition.y
            };

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

                position.x -= matrix4X4.M41;
                position.y -= matrix4X4.M42;
            }

            if (IsFreeDraggable)
            {
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
                    left = screenWidth - parentWidth;

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
            }

            OnDragPanel(left, top);
        }

        public virtual void OnDragPanel(float left, float top)
        {
            float scale = DragBasePanel.ScaleToScreen;

            DragBasePanel.Style.Left = Length.Pixels(left / scale);
            DragBasePanel.Style.Top = Length.Pixels(top / scale);
        }

        public virtual void OnDragPanelFinished()
        {

        }

        public virtual void OnDragPanelSuccess(Drop targetDrop, int? index = null)
        {
            if (targetDrop != null)
            {
                targetDrop.AddChild(this);

                if (!IsFreeDraggable)
                {
                    DragBasePanel.Style.Left = null;
                    DragBasePanel.Style.Top = null;
                }
            }
        }

        public virtual void OnDragPanelFailed(Drop targetDrop)
        {
            DragBasePanel.Style.Left = _oldPositionLeft;
            DragBasePanel.Style.Top = _oldPositionTop;
        }

        private static Drop GetDropPanel()
        {
            foreach (Drop drop in Drop.List)
            {
                if (drop.IsInside(Mouse.Position))
                {
                    return drop;
                }
            }

            return null;
        }
    }
}
