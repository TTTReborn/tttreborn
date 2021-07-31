using System;
using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class ResizeablePanel : Panel
    {
        public enum DragAnchor {
            TOPLEFT,
            TOP,
            TOPRIGHT,
            RIGHT,
            BOTTOMRIGHT,
            BOTTOM,
            BOTTOMLEFT,
            LEFT
        }

        public bool IsDragging { get; private set; } = false;

        public bool CanStartDragging
        {
            get
            {
                return _canStartDragging;
            }
            private set
            {
                _canStartDragging = value;

                SetClass("draggable", _canStartDragging);
            }
        }
        private bool _canStartDragging = false;

        public DragAnchor? CurrentDragAnchor
        {
            get
            {
                return _currentDragAnchor;
            }
            private set
            {
                _currentDragAnchor = value;

                CanStartDragging = _currentDragAnchor != null;
            }
        }

        private DragAnchor? _currentDragAnchor = null;

        public const float DRAG_OBLIGINGNESS = 16f;

        private Vector2 _draggingMouseStartPosition;
        private Box _boxBeforeDraggingStarted;

        public ResizeablePanel() : base()
        {
            StyleSheet.Load("/ui/generalhud/menu/ResizeablePanel.scss");

            //AcceptsFocus = true;
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            if (!IsVisible || !CanStartDragging)
            {
                return;
            }

            _draggingMouseStartPosition = MousePosition;
            _boxBeforeDraggingStarted = Box;

            IsDragging = true;
        }

        protected override void OnMouseUp(MousePanelEvent e)
        {
            base.OnMouseUp(e);

            IsDragging = false;
        }

        protected override void OnMouseMove(MousePanelEvent e)
        {
            base.OnMouseMove(e);

            if (!IsDragging)
            {
                return;
            }

            float screenWidth = Screen.Width;
            float screenHeight = Screen.Height;

            Vector2 delta = new Vector2(
                (MousePosition.x - _draggingMouseStartPosition.x),
                (MousePosition.y - _draggingMouseStartPosition.y)
            );

            float width = Box.Rect.width;
            float height = Box.Rect.height;
            float x = Box.Left;
            float y = Box.Top;

            switch (CurrentDragAnchor)
            {
                case DragAnchor.LEFT:
                    width = _boxBeforeDraggingStarted.Rect.width - delta.x;
                    x = _boxBeforeDraggingStarted.Rect.left + delta.x;

                    break;
                case DragAnchor.RIGHT:
                    width = _boxBeforeDraggingStarted.Rect.width + delta.x;
                    height = _boxBeforeDraggingStarted.Rect.height + delta.y;

                    break;
            }

            float minWidth = ComputedStyle.MinWidth != null ? ComputedStyle.MinWidth.Value.GetPixels(screenWidth) : width;
            float maxWidth = ComputedStyle.MaxWidth != null ? ComputedStyle.MaxWidth.Value.GetPixels(screenWidth) : width;
            float minHeight = ComputedStyle.MinHeight != null ? ComputedStyle.MinHeight.Value.GetPixels(screenHeight) : height;
            float maxHeight = ComputedStyle.MaxHeight != null ? ComputedStyle.MaxHeight.Value.GetPixels(screenHeight) : height;

            if (minWidth > width)
            {
                width = minWidth;
                x = _boxBeforeDraggingStarted.Rect.right - minWidth;
            }

            if (maxWidth < width)
            {
                width = maxWidth;
                x = _boxBeforeDraggingStarted.Rect.right - maxWidth;
            }

            if (minHeight > height)
            {
                height = minHeight;
                y = _boxBeforeDraggingStarted.Rect.bottom - minHeight;
            }

            if (maxHeight < height)
            {
                height = maxHeight;
                y = _boxBeforeDraggingStarted.Rect.bottom - maxHeight;
            }

            Style.Width = Length.Pixels(width);
            Style.Height = Length.Pixels(height);
            Style.Left = Length.Pixels(x);
            Style.Top = Length.Pixels(y);
            Style.Dirty();
        }

        public override void Tick()
        {
            if (!IsVisible || ComputedStyle == null || IsDragging)
            {
                return;
            }

            if (MousePosition.x > 0f && MousePosition.x < DRAG_OBLIGINGNESS)
            {
                CurrentDragAnchor = DragAnchor.LEFT;
            }
            else
            {
                CurrentDragAnchor = null;
            }
        }
    }
}
