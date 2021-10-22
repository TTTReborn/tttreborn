using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    /// <summary>
    /// A horizontal slider. Can be float or whole number.
    /// </summary>
    public class Slider : Panel
    {
        public Panel Track { get; protected set; }
        public Panel TrackInner { get; protected set; }
        public Panel Thumb { get; protected set; }

        /// <summary>
        /// The right side of the slider
        /// </summary>
        public float MaxValue { get; set; } = 100;

        /// <summary>
        /// The left side of the slider
        /// </summary>
        public float MinValue { get; set; } = 0;

        /// <summary>
        /// If set to 1, value will be rounded to 1's
        /// If set to 10, value will be rounded to 10's
        /// If set to 0.1, value will be rounded to 0.1's
        /// </summary>
        public float Step { get; set; } = 1.0f;

        public Slider(Sandbox.UI.Panel parent) : base(parent)
        {
            StyleSheet.Load("ui/components/slider/Slider.scss");

            AddClass("slider");

            Track = new();
            Track.AddClass("track");

            TrackInner = new();
            TrackInner.AddClass("inner");

            Thumb = new();
            Thumb.AddClass("thumb");
        }

        protected float _value = float.MaxValue;

        /// <summary>
        /// The actual value. Setting the value will snap and clamp it.
        /// </summary>
        public float Value
        {
            get => _value.Clamp(MinValue, MaxValue);
            set
            {
                float snapped = Step > 0 ? value.SnapToGrid(Step) : value;
                snapped = snapped.Clamp(MinValue, MaxValue);

                if (_value == snapped) return;

                _value = snapped;

                CreateValueEvent("value", _value);
                UpdateSliderPositions();
            }
        }

        public override void SetProperty(string name, string value)
        {
            if (name == "min" && float.TryParse(value, out float floatValue))
            {
                MinValue = floatValue;
                UpdateSliderPositions();
                return;
            }

            if (name == "step" && float.TryParse(value, out floatValue))
            {
                Step = floatValue;
                UpdateSliderPositions();
                return;
            }

            if (name == "max" && float.TryParse(value, out floatValue))
            {
                MaxValue = floatValue;
                UpdateSliderPositions();
                return;
            }

            if (name == "value" && float.TryParse(value, out floatValue))
            {
                Value = floatValue;
                return;
            }

            base.SetProperty(name, value);
        }

        /// <summary>
        /// Convert a screen position to a value. The value is clamped, but not snapped.
        /// </summary>
        public virtual float ScreenPosToValue(Vector2 pos)
        {
            Vector2 localPos = ScreenPositionToPanelPosition(pos);
            float thumbSize = Thumb.Box.Rect.width * 0.5f;
            float normalized = MathX.LerpInverse(localPos.x, thumbSize, Box.Rect.width - thumbSize, true);
            float scaled = MathX.LerpTo(MinValue, MaxValue, normalized, true);
            return Step > 0 ? scaled.SnapToGrid(Step) : scaled;
        }

        /// <summary>
        /// If we move the mouse while we're being pressed then set the position,
        /// but skip transitions.
        /// </summary>
        protected override void OnMouseMove(MousePanelEvent e)
        {
            base.OnMouseMove(e);

            if (!HasActive) return;

            Value = ScreenPosToValue(Mouse.Position);
            UpdateSliderPositions();
            SkipTransitions();
            e.StopPropagation();
        }

        /// <summary>
        /// On mouse press jump to that position
        /// </summary>
        protected override void OnMouseDown(MousePanelEvent e)
        {
            base.OnMouseDown(e);

            Value = ScreenPosToValue(Mouse.Position);
            UpdateSliderPositions();
            e.StopPropagation();
        }

        int positionHash;

        /// <summary>
        /// Updates the styles for TrackInner and Thumb to position us based on the current value.
        /// Note this purposely uses percentages instead of pixels when setting up, this way we don't
        /// have to worry about parent size, screen scale etc.
        /// </summary>
        void UpdateSliderPositions()
        {
            int hash = HashCode.Combine(Value, MinValue, MaxValue);
            if (hash == positionHash) return;

            positionHash = hash;

            float pos = MathX.LerpInverse(Value, MinValue, MaxValue, true);

            TrackInner.Style.Width = Length.Fraction(pos);
            Thumb.Style.Left = Length.Fraction(pos);

            TrackInner.Style.Dirty();
            Thumb.Style.Dirty();
        }

    }

    namespace Construct
    {
        public static class SliderConstructor
        {
            public static Slider Slider(this PanelCreator self, float min, float max, float step)
            {
                Slider control = new(self.panel);
                control.MinValue = min;
                control.MaxValue = max;
                control.Step = step;

                return control;
            }
        }
    }
}
