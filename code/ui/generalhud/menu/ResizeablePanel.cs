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

        public DragAnchor? CurrentHorizontalDragAnchor
        {
            get
            {
                return _currentHorizontalDragAnchor;
            }
            private set
            {
                _currentHorizontalDragAnchor = value;

                if (value != null)
                {
                    if (CurrentDragAnchor == DragAnchor.TOP)
                    {
                        CurrentDragAnchor = value == DragAnchor.LEFT ? DragAnchor.TOPLEFT : DragAnchor.TOPRIGHT;
                    }
                    else if (CurrentDragAnchor == DragAnchor.BOTTOM)
                    {
                        CurrentDragAnchor = value == DragAnchor.LEFT ? DragAnchor.BOTTOMLEFT : DragAnchor.BOTTOMRIGHT;
                    }
                    else
                    {
                        CurrentDragAnchor = value;
                    }
                }
                else
                {
                    CurrentDragAnchor = CurrentVerticalDragAnchor;
                }
            }
        }
        private DragAnchor? _currentHorizontalDragAnchor = null;

        public DragAnchor? CurrentVerticalDragAnchor
        {
            get
            {
                return _currentVerticalDragAnchor;
            }
            private set
            {
                _currentVerticalDragAnchor = value;

                if (value != null)
                {
                    if (CurrentDragAnchor == DragAnchor.LEFT)
                    {
                        CurrentDragAnchor = value == DragAnchor.TOP ? DragAnchor.TOPLEFT : DragAnchor.BOTTOMLEFT;
                    }
                    else if (CurrentDragAnchor == DragAnchor.RIGHT)
                    {
                        CurrentDragAnchor = value == DragAnchor.TOP ? DragAnchor.TOPRIGHT : DragAnchor.BOTTOMRIGHT;
                    }
                    else
                    {
                        CurrentDragAnchor = value;
                    }
                }
                else
                {
                    CurrentDragAnchor = CurrentHorizontalDragAnchor;
                }
            }
        }
        private DragAnchor? _currentVerticalDragAnchor = null;

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
            float left = Box.Rect.left;
            float top = Box.Rect.top;

            float minWidth = ComputedStyle.MinWidth != null ? ComputedStyle.MinWidth.Value.GetPixels(screenWidth) : width;
            float maxWidth = ComputedStyle.MaxWidth != null ? ComputedStyle.MaxWidth.Value.GetPixels(screenWidth) : width;
            float minHeight = ComputedStyle.MinHeight != null ? ComputedStyle.MinHeight.Value.GetPixels(screenHeight) : height;
            float maxHeight = ComputedStyle.MaxHeight != null ? ComputedStyle.MaxHeight.Value.GetPixels(screenHeight) : height;

            switch (CurrentHorizontalDragAnchor)
            {
                case DragAnchor.LEFT:
                    width = Box.Rect.width - delta.x;
                    left = Box.Rect.left + delta.x;

                    break;
                case DragAnchor.RIGHT:
                    width = _boxDataBeforeDraggingStarted.Width + delta.x;

                    break;
            }

            switch (CurrentVerticalDragAnchor)
            {
                case DragAnchor.TOP:
                    height = Box.Rect.height - delta.y;
                    top = Box.Rect.top + delta.y;

                    break;
                case DragAnchor.BOTTOM:
                    height = _boxDataBeforeDraggingStarted.Height + delta.y;

                    break;
            }

            if (minWidth > width)
            {
                width = minWidth;

                if (CurrentHorizontalDragAnchor == DragAnchor.LEFT)
                {
                    left = _boxDataBeforeDraggingStarted.Right - minWidth;
                }
            }

            if (maxWidth < width)
            {
                width = maxWidth;

                if (CurrentHorizontalDragAnchor == DragAnchor.LEFT)
                {
                    left = _boxDataBeforeDraggingStarted.Right - maxWidth;
                }
            }

            if (minHeight > height)
            {
                height = minHeight;

                if (CurrentVerticalDragAnchor == DragAnchor.TOP)
                {
                    top = _boxDataBeforeDraggingStarted.Bottom - minHeight;
                }
            }

            if (maxHeight < height)
            {
                height = maxHeight;

                if (CurrentVerticalDragAnchor == DragAnchor.TOP)
                {
                    top = _boxDataBeforeDraggingStarted.Bottom - maxHeight;
                }
            }

            Style.Width = Length.Pixels(width);
            Style.Height = Length.Pixels(height);
            Style.Left = Length.Pixels(left);
            Style.Top = Length.Pixels(top);
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
                CurrentHorizontalDragAnchor = DragAnchor.LEFT;
            }
            else if (MousePosition.x < Box.Rect.width && MousePosition.x > Box.Rect.width - DRAG_OBLIGINGNESS)
            {
                CurrentHorizontalDragAnchor = DragAnchor.RIGHT;
            }
            else
            {
                CurrentHorizontalDragAnchor = null;
            }

            if (MousePosition.y > 0f && MousePosition.y < DRAG_OBLIGINGNESS)
            {
                CurrentVerticalDragAnchor = DragAnchor.TOP;
            }
            else if (MousePosition.y < Box.Rect.height && MousePosition.y > Box.Rect.height - DRAG_OBLIGINGNESS)
            {
                CurrentVerticalDragAnchor = DragAnchor.BOTTOM;
            }
            else
            {
                CurrentVerticalDragAnchor = null;
            }
        }
    }
}
