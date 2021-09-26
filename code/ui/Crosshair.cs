using System;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public class Crosshair : Panel
    {
        public static Crosshair Current;

        public bool ShowDot
        {
            get => _showDot;
            set { _showDot = value; UpdateCrosshair(); }
        }
        private bool _showDot = true;
        public bool ShowTop
        {
            get => _showTop;
            set { _showTop = value; UpdateCrosshair(); }
        }
        private bool _showTop = true;
        public bool ShowOutline
        {
            get => _showOutline;
            set { _showOutline = value; UpdateCrosshair(); }
        }
        private bool _showOutline = true;
        public int Thickness
        {
            get => _thickness;
            set { _thickness = value; UpdateCrosshair(); }
        }
        private int _thickness = 4;
        public int Size
        {
            get => _size;
            set { _size = value; UpdateCrosshair(); }
        }
        private int _size = 0;
        public int OutlineThickness
        {
            get => _outlineThickness;
            set { _outlineThickness = value; UpdateCrosshair(); }
        }
        private int _outlineThickness = 0;
        public int OutlineBlur
        {
            get => _outlineBlur;
            set { _outlineBlur = value; UpdateCrosshair(); }
        }
        private int _outlineBlur = 4;
        public int Gap
        {
            get => _gap;
            set { _gap = value; UpdateCrosshair(); }
        }
        private int _gap = 6;
        public Color Color
        {
            get => _color;
            set { _color = value; UpdateCrosshair(); }
        }
        private Color _color = Color.White;

        private Panel _crosshairDot;
        private Panel[] _crosshairLines;

        public Crosshair()
        {
            Current = this;

            AddClass("centered");

            _crosshairDot = new Panel(this);
            _crosshairDot.AddClass("circular");

            _crosshairLines = new Panel[4];
            for (int i = 0; i < _crosshairLines.Length; i++)
            {
                _crosshairLines[i] = new Panel(this);
                _crosshairLines[i].AddClass("centered");
            }

            UpdateCrosshair();
        }

        public void UpdateCrosshair()
        {
            Shadow shadow = new Shadow();
            shadow.OffsetX = 0;
            shadow.OffsetY = 0;
            shadow.Blur = OutlineBlur;
            shadow.Spread = OutlineThickness;
            shadow.Color = Color.Black;

            #region Update Crosshair Dot
            _crosshairDot.Enabled = ShowDot;

            if (ShowDot)
            {
                _crosshairDot.Style.BackgroundColor = Color;

                _crosshairDot.Style.Width = Thickness;
                _crosshairDot.Style.Height = Thickness;

                if (ShowOutline)
                {
                    _crosshairDot.Style.BoxShadow.Add(shadow);
                }

                _crosshairDot.Style.Dirty();
            }
            #endregion

            #region Update Crosshair Lines
            for (int i = 0; i < _crosshairLines.Length; i++)
            {
                bool isHorizontal = i % 2 == 0;

                _crosshairLines[i].Style.BackgroundColor = Color;
                _crosshairLines[i].Style.Width = isHorizontal ? Size : Thickness;
                _crosshairLines[i].Style.Height = isHorizontal ? Thickness : Size;

                switch (i)
                {
                    case 0: // Left
                        _crosshairLines[i].Style.MarginLeft = -(Size / 2 + Gap);
                        break;
                    case 1: // Bottom
                        _crosshairLines[i].Style.MarginTop = (Size / 2 + Gap);
                        break;
                    case 2: // Right
                        _crosshairLines[i].Style.MarginLeft = (Size / 2 + Gap);
                        break;
                    case 3: // Top
                        _crosshairLines[i].Enabled = ShowTop;
                        _crosshairLines[i].Style.MarginTop = -(Size / 2 + Gap);
                        break;
                }

                if (ShowOutline && Size > 0)
                {
                    _crosshairLines[i].Style.BoxShadow.Add(shadow);
                }

                _crosshairLines[i].Style.Dirty();
            }
            #endregion

            Style.Dirty();
        }
    }
}
