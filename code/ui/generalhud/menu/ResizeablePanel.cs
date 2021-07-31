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

        public const float DRAG_OBLIGINGNESS = 8f;

        private Vector2 _draggingMouseStartPosition;
        private Vector2 _sizeBeforeDraggingStarted;

        public ResizeablePanel() : base()
        {
            StyleSheet.Load("/ui/generalhud/menu/ResizeablePanel.scss");
        }

        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            if (!IsVisible || !CanStartDragging)
            {
                return;
            }

            _draggingMouseStartPosition = MousePosition;
            _sizeBeforeDraggingStarted = new Vector2(
                ComputedStyle.Width.Value.GetPixels(Screen.Width),
                ComputedStyle.Height.Value.GetPixels(Screen.Height)
            );

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

            Vector2 position = new Vector2(
                ComputedStyle.Left.Value.GetPixels(screenWidth),
                ComputedStyle.Top.Value.GetPixels(screenHeight)
            );

            Vector2 size = new Vector2(
                ComputedStyle.Width.Value.GetPixels(screenWidth),
                ComputedStyle.Height.Value.GetPixels(screenHeight)
            );

            float width = size.x;
            float height = size.y;
            float x = position.x;
            float y = position.y;

            switch (CurrentDragAnchor)
            {
                case DragAnchor.LEFT:
                    width = size.x - delta.x;
                    x = position.x + delta.x;

                    break;
                case DragAnchor.RIGHT:
                    width = _sizeBeforeDraggingStarted.x + delta.x;
                    height = _sizeBeforeDraggingStarted.y + delta.y;

                    break;
            }

            // TODO improve fast mouse dragging (set x to min left and width to max width etc.)
            if (ComputedStyle.MinWidth != null && ComputedStyle.MinWidth.Value.GetPixels(screenWidth) > width)
            {
                return;
            }

            if (ComputedStyle.MaxWidth != null && ComputedStyle.MaxWidth.Value.GetPixels(screenWidth) < width)
            {
                return;
            }

            if (ComputedStyle.MinHeight != null && ComputedStyle.MinHeight.Value.GetPixels(screenHeight) > height)
            {
                return;
            }

            if (ComputedStyle.MaxHeight != null && ComputedStyle.MaxHeight.Value.GetPixels(screenHeight) < height)
            {
                return;
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

            float screenWidth = Screen.Width;
            float screenHeight = Screen.Height;

            Vector2 size = new Vector2(
                ComputedStyle.Width.Value.GetPixels(screenWidth),
                ComputedStyle.Height.Value.GetPixels(screenHeight)
            );

            Vector2 position = new Vector2(
                ComputedStyle.Left.Value.GetPixels(screenWidth),
                ComputedStyle.Top.Value.GetPixels(screenHeight)
            );

            if (MousePosition.x > -DRAG_OBLIGINGNESS && MousePosition.x < DRAG_OBLIGINGNESS)
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
