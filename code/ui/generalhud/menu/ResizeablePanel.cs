using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public struct BoxData
    {
        public float Width;
        public float Height;
        public float Top;
        public float Right;
        public float Bottom;
        public float Left;
    }

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
        private BoxData _boxDataBeforeDraggingStarted;

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
            _boxDataBeforeDraggingStarted = new BoxData
            {
                Width = Box.Rect.width,
                Height = Box.Rect.height,
                Top = Box.Rect.top,
                Right = Box.Rect.right,
                Bottom = Box.Rect.bottom,
                Left = Box.Rect.left
            };

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

            Vector2 delta = new Vector2(
                (MousePosition.x - _draggingMouseStartPosition.x),
                (MousePosition.y - _draggingMouseStartPosition.y)
            );

            float screenWidth = Screen.Width;
            float screenHeight = Screen.Height;

            float width = Box.Rect.width;
            float height = Box.Rect.height;
            float x = Box.Rect.left;
            float y = Box.Rect.top;

            float minWidth = ComputedStyle.MinWidth != null ? ComputedStyle.MinWidth.Value.GetPixels(screenWidth) : width;
            float maxWidth = ComputedStyle.MaxWidth != null ? ComputedStyle.MaxWidth.Value.GetPixels(screenWidth) : width;
            float minHeight = ComputedStyle.MinHeight != null ? ComputedStyle.MinHeight.Value.GetPixels(screenHeight) : height;
            float maxHeight = ComputedStyle.MaxHeight != null ? ComputedStyle.MaxHeight.Value.GetPixels(screenHeight) : height;

            switch (CurrentDragAnchor)
            {
                case DragAnchor.LEFT:
                    width = Box.Rect.width - delta.x;
                    x = Box.Rect.left + delta.x;

                    break;
                case DragAnchor.RIGHT:
                    width = _boxDataBeforeDraggingStarted.Width + delta.x;

                    break;
            }

            if (minWidth > width)
            {
                width = minWidth;

                if (CurrentDragAnchor == DragAnchor.LEFT)
                {
                    x = _boxDataBeforeDraggingStarted.Right - minWidth;
                }
            }

            if (maxWidth < width)
            {
                width = maxWidth;

                if (CurrentDragAnchor == DragAnchor.LEFT)
                {
                    x = _boxDataBeforeDraggingStarted.Right - maxWidth;
                }
            }

            if (minHeight > height)
            {
                height = minHeight;

                if (CurrentDragAnchor == DragAnchor.TOP)
                {
                    y = _boxDataBeforeDraggingStarted.Bottom - minHeight;
                }
            }

            if (maxHeight < height)
            {
                height = maxHeight;

                if (CurrentDragAnchor == DragAnchor.TOP)
                {
                    y = _boxDataBeforeDraggingStarted.Bottom - maxHeight;
                }
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
            else if (MousePosition.x < Box.Rect.width && MousePosition.x > Box.Rect.width - DRAG_OBLIGINGNESS)
            {
                CurrentDragAnchor = DragAnchor.RIGHT;
            }
            else
            {
                CurrentDragAnchor = null;
            }
        }
    }
}
