using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Map;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class LogicButtonPoint : Panel
    {
        // Our data we received initially from the server during creation.
        public TTTLogicButtonData Data { get; private set; }

        // Our specific assigned Entity.
        private readonly TTTLogicButton _entity;

        // Position pulled from Data
        public Vector3 Position { get; private set; }

        // If the distance from the player to the button is less than this value, the element is fully visible.
        private static int _minViewDistance = 512;

        // Between MINVIEWDISTANCE and this value, the element will slowly become transparent.
        // Past this distance, the button is unusuable.
        private readonly int _maxViewDistance = 1024;

        public LogicButtonPoint(TTTLogicButtonData data)
        {
            Data = data;
            Position = data.Position;
            _maxViewDistance = data.Range;
            _minViewDistance = Math.Min(_minViewDistance, _maxViewDistance / 2);

            StyleSheet.Load("/map/logicbutton/LogicButtonPoint.scss");

            Hud.Current.RootPanel.AddChild(this);

            _entity = Entity.FindByIndex(Data.NetworkIdent) as TTTLogicButton;

            _ = Add.Label(_entity.Description);
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Vector3 screenPos = Position.ToScreen();
            this.Enabled(screenPos.z > 0f);

            // If our entity is locked, delayed or removed, let's not show it.
            if (_entity.IsDisabled)
            {
                Style.Display = DisplayMode.None;

                // Make sure our client is no longer tracking this element.
                if (TTTPlayer.FocusedButton == this)
                {
                    TTTPlayer.FocusedButton = null;
                }

                this.Enabled(false);

                return;
            }

            if (this.IsEnabled())
            {
                Style.Display = DisplayMode.Flex;
                Style.Left = Length.Fraction(screenPos.x);
                Style.Top = Length.Fraction(screenPos.y);
                Style.Opacity = Math.Clamp(1f - (player.Position.Distance(Position) - _minViewDistance) / (_maxViewDistance - _minViewDistance), 0f, 1f);

                // Update our 'focus' CSS look if our player currently is looking near this point.
                SetClass("focus", TTTPlayer.FocusedButton == this);

                // Check if point is within 10% of the crosshair.
                if (IsLengthWithinCamerasFocus() && player.Position.Distance(Position) <= _maxViewDistance)
                {
                    TTTPlayer.FocusedButton ??= this; // If the current focused button is null, update it to this.
                }
                else if (TTTPlayer.FocusedButton == this) // If we are the current focused button, but we are out of focus, set to null
                {
                    TTTPlayer.FocusedButton = null;
                }
            }
        }

        // Our "screen focus" size, roughly %5 of the screen around the cross hair.
        // It might be worth considering using an alternate method to percentages for larger screens. Hoping we can test that with someone who has a UHD monitor.
        private const float FOCUS_SIZE = 2.5f;
        private const int CENTER_PERCENT = 50;

        public bool IsLengthWithinCamerasFocus()
        {
            // We have to adjust the top check by the screen's aspect ratio in order to compensate for screen size
            float topHeight = FOCUS_SIZE * Screen.Aspect;

            // I think we could alternatively use
            return Style.Left.Value.Value > CENTER_PERCENT - FOCUS_SIZE && Style.Left.Value.Value < CENTER_PERCENT + FOCUS_SIZE
                && Style.Top.Value.Value > CENTER_PERCENT - topHeight && Style.Top.Value.Value < CENTER_PERCENT + topHeight;
        }

        // Check to make sure player is within range and our button is not disabled.
        // Called when client calls for button to be activated. A simple double check.
        public bool IsUsable(TTTPlayer player)
        {
            return player.Position.Distance(Position) <= Data.Range && !Data.IsDisabled;
        }
    }
}
