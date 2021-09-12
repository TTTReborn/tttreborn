using System;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public class Crosshair : Panel
    {
        public static Crosshair Current;

        private bool _showDot = true;
        public bool ShowDot
        {
            get { return _showDot; }
            set { _showDot = value; UpdateCrosshair(); }
        }
        private bool _showTop = true;
        public bool ShowTop
        {
            get { return _showTop; }
            set { _showTop = value; UpdateCrosshair(); }
        }
        private bool _showOutline = true;
        public bool ShowOutline
        {
            get { return _showOutline; }
            set { _showOutline = value; UpdateCrosshair(); }
        }
        private int _thickness = 4;
        public int Thickness
        {
            get { return _thickness; }
            set { _thickness = value; UpdateCrosshair(); }
        }
        private int _size = 0;
        public int Size
        {
            get { return _size; }
            set { _size = value; UpdateCrosshair(); }
        }
        private int _outlineThickness = 0;
        public int OutlineThickness
        {
            get { return _outlineThickness; }
            set { _outlineThickness = value; UpdateCrosshair(); }
        }
        private int _outlineBlur = 4;
        public int OutlineBlur
        {
            get { return _outlineBlur; }
            set { _outlineBlur = value; UpdateCrosshair(); }
        }
        private int _gap = 6;
        public int Gap
        {
            get { return _gap; }
            set { _gap = value; UpdateCrosshair(); }
        }
        private Color _color = Color.White;
        public Color Color
        {
            get { return _color; }
            set { _color = value; UpdateCrosshair(); }
        }

        private Panel _crosshairDot;
        private Panel[] _crosshairLines;

        private Panel ChargeBar;
        private float ChargeTime;

        public Crosshair()
        {
            Current = this;

            AddClass("centered");

            _crosshairDot = new Panel(this);
            _crosshairDot.AddClass("centered");
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
