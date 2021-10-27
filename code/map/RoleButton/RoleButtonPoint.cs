using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Map;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class RoleButtonPoint : Panel
    {
        // Our data we received initially from the server during creation.
        public TTTRoleButtonData Data { get; private set; }

        // Our specific assigned Entity.
        private TTTRoleButton _entity;

        // Position pulled from Data
        public Vector3 Position { get; private set; }

        private readonly Label _descriptionLabel;

        // If the distance from the player to the button is less than this value, the element is fully visible.
        private const int MIN_VIEW_DISTANCE = 512;

        // Between MINVIEWDISTANCE and this value, the element will slowly become transparent.
        // Past this distance, the button is unusuable.
        private readonly int _maxViewDistance = 1024;

        public RoleButtonPoint(TTTRoleButtonData data)
        {
            Data = data;
            Position = data.Position;
            _maxViewDistance = data.Range;

            StyleSheet.Load("/map/RoleButton/RoleButtonPoint.scss");

            Hud.Current.RootPanel.AddChild(this);

            _entity = Entity.FindByIndex(Data.NetworkIdent) as TTTRoleButton;

            _descriptionLabel = Add.Label(_entity.Description);
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Vector3 screenPos = Position.ToScreen();
            Enabled = screenPos.z > 0f;

            // If our entity is locked, delayed or removed, let's not show it.
            if (_entity.IsDisabled)
            {
                Style.Display = DisplayMode.None;
                Style.Dirty();

                // Make sure our client is no longer tracking this element.
                if (TTTPlayer.FocusedButton == this)
                {
                    TTTPlayer.FocusedButton = null;
                }

                Enabled = false;

                return;
            }

            if (Enabled)
            {
                Style.Display = DisplayMode.Flex;
                Style.Left = Length.Fraction(screenPos.x);
                Style.Top = Length.Fraction(screenPos.y);
                Style.Opacity = Math.Clamp(1f - (player.Position.Distance(Position) - MIN_VIEW_DISTANCE) / (_maxViewDistance - MIN_VIEW_DISTANCE), 0f, 1f);

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

                Style.Dirty();
            }
        }

        // Our "screen focus" size, roughly %5 of the screen around the cross hair.
        // It might be worth considering using an alternate method to percentages for larger screens. Hoping we can test that with someone who has a UHD monitor.
        private float _focusSize = 2.5f;
        private const int _centerPercent = 50;

        public bool IsLengthWithinCamerasFocus()
        {
            // We have to adjust the top check by the screen's aspect ratio in order to compensate for screen size
            float topHeight = _focusSize * Screen.Aspect;

            // I think we could alternatively use
            return Style.Left.Value.Value > _centerPercent - _focusSize && Style.Left.Value.Value < _centerPercent + _focusSize
                && Style.Top.Value.Value > _centerPercent - topHeight && Style.Top.Value.Value < _centerPercent + topHeight;
        }

        // Check to make sure player is within range and our button is not disabled.
        // Called when client calls for button to be activated. A simple double check.
        public bool IsUsable(TTTPlayer player)
        {
            return player.Position.Distance(Position) <= Data.Range && !Data.IsDisabled;
        }
    }
}
