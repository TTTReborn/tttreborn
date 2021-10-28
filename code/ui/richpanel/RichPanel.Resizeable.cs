using System;

using Sandbox;
using Sandbox.UI;

// TODO use M4x4 transform

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
        public Vector2? TransformRatio;
    }

    public partial class RichPanel : Panel
    {
        public bool IsDraggable = false;

        public enum DragAnchor
        {
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
            get => _canStartDragging && IsDraggable;
            private set
            {
                _canStartDragging = value;

                SetClass("draggable", CanStartDragging);
            }
        }
        private bool _canStartDragging = false;

        public DragAnchor? CurrentDragAnchor
        {
            get => _currentDragAnchor;
            private set
            {
                _currentDragAnchor = value;

                CanStartDragging = _currentDragAnchor != null;
            }
        }
        private DragAnchor? _currentDragAnchor = null;

        public DragAnchor? CurrentHorizontalDragAnchor
        {
            get => _currentHorizontalDragAnchor;
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
            get => _currentVerticalDragAnchor;
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

        public const float DRAG_OBLIGINGNESS = 12f;

        private Vector2 _draggingMouseStartPosition;
        private BoxData _boxDataBeforeDraggingStarted;

        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            if (!IsVisible || !CanStartDragging || IsDragging)
            {
                return;
            }

            _draggingMouseStartPosition = Mouse.Position;
            _boxDataBeforeDraggingStarted = new BoxData
            {
                Width = (float) (Math.Ceiling(Box.Rect.width)),
                Height = (float) (Math.Ceiling(Box.Rect.height)),
                Top = (float) (Math.Ceiling(Box.Rect.top)),
                Right = (float) (Math.Ceiling(Box.Rect.right)),
                Bottom = (float) (Math.Ceiling(Box.Rect.bottom)),
                Left = (float) (Math.Ceiling(Box.Rect.left))
            };

            Matrix? matrix = GlobalMatrix;

            if (matrix != null)
            {
                _boxDataBeforeDraggingStarted.TransformRatio = new Vector2(
                    (float) Math.Ceiling(matrix.Value.Numerics.M41) / _boxDataBeforeDraggingStarted.Width,
                    (float) Math.Ceiling(matrix.Value.Numerics.M42) / _boxDataBeforeDraggingStarted.Height
                );
            }
            else
            {
                _boxDataBeforeDraggingStarted.TransformRatio = new Vector2(0f, 0f);
            }

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
                Mouse.Position.x - _draggingMouseStartPosition.x,
                Mouse.Position.y - _draggingMouseStartPosition.y
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
                    width = _boxDataBeforeDraggingStarted.Width - delta.x;
                    left = _boxDataBeforeDraggingStarted.Left + delta.x - delta.x * _boxDataBeforeDraggingStarted.TransformRatio.Value.x;

                    break;
                case DragAnchor.RIGHT:
                    width = _boxDataBeforeDraggingStarted.Width + delta.x;
                    left = _boxDataBeforeDraggingStarted.Left + delta.x * _boxDataBeforeDraggingStarted.TransformRatio.Value.x;

                    break;
            }

            switch (CurrentVerticalDragAnchor)
            {
                case DragAnchor.TOP:
                    height = _boxDataBeforeDraggingStarted.Height - delta.y;
                    top = _boxDataBeforeDraggingStarted.Top + delta.y - delta.y * _boxDataBeforeDraggingStarted.TransformRatio.Value.y;

                    break;
                case DragAnchor.BOTTOM:
                    height = _boxDataBeforeDraggingStarted.Height + delta.y;
                    top = _boxDataBeforeDraggingStarted.Top + delta.y * _boxDataBeforeDraggingStarted.TransformRatio.Value.y;

                    break;
            }

            // This fixes +-1f jiggling
            if (Math.Ceiling(Math.Ceiling(width * _boxDataBeforeDraggingStarted.TransformRatio.Value.x) / _boxDataBeforeDraggingStarted.TransformRatio.Value.x) != width)
            {
                if (CurrentHorizontalDragAnchor == DragAnchor.RIGHT)
                {
                    left -= 1f;
                }
                else if (CurrentHorizontalDragAnchor == DragAnchor.LEFT)
                {
                    width -= 1f;
                }
            }

            if (Math.Ceiling(Math.Ceiling(height * _boxDataBeforeDraggingStarted.TransformRatio.Value.y) / _boxDataBeforeDraggingStarted.TransformRatio.Value.y) != height)
            {
                if (CurrentVerticalDragAnchor == DragAnchor.BOTTOM)
                {
                    top -= 1f;
                }
                else if (CurrentVerticalDragAnchor == DragAnchor.TOP)
                {
                    height -= 1f;
                }
            }

            if (minWidth > width || maxWidth < width)
            {
                width = minWidth > width ? minWidth : maxWidth;

                float deltaWidth = _boxDataBeforeDraggingStarted.Width - width;

                left = _boxDataBeforeDraggingStarted.Left + deltaWidth - deltaWidth * _boxDataBeforeDraggingStarted.TransformRatio.Value.x;

                if (CurrentHorizontalDragAnchor == DragAnchor.RIGHT)
                {
                    left -= deltaWidth;
                }
            }

            if (minHeight > height || maxHeight < height)
            {
                height = minHeight > height ? minHeight : maxHeight;

                float deltaHeight = _boxDataBeforeDraggingStarted.Height - height;

                top = _boxDataBeforeDraggingStarted.Top + deltaHeight - deltaHeight * _boxDataBeforeDraggingStarted.TransformRatio.Value.y;

                if (CurrentVerticalDragAnchor == DragAnchor.BOTTOM)
                {
                    top -= deltaHeight;
                }
            }

            Style.Width = Length.Pixels((float) Math.Ceiling(width));
            Style.Height = Length.Pixels((float) Math.Ceiling(height));
            Style.Left = Length.Pixels((float) Math.Ceiling(left));
            Style.Top = Length.Pixels((float) Math.Ceiling(top));
            Style.Dirty();
        }

        public override void Tick()
        {
            base.Tick();

            if (!IsVisible || ComputedStyle == null || IsDragging || !IsDraggable)
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
