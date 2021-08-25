
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Map;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class RoleButtonPoint : TTTPanel
    {
        //Our data we received initially from the server during creation.
        public TTTRoleButtonData Data { get; private set; }

        //Our specific assigned Entity.
        private TTTRoleButton Entity;

        //Position pulled from Data
        public Vector3 Position { get; private set; }

        private Label DescriptionLabel; 

        //If the distance from the player to the button is less than this value, the element is fully visible.
        private const int MINVIEWDISTANCE = 512;
        //Between MINVIEWDISTANCE and this value, the element will slowly become transparent.
        //Past this distance, the button is unusuable.
        private readonly int MaxViewDistance = 1024;

        public RoleButtonPoint(TTTRoleButtonData data)
        {
            Data = data;
            Position = data.Position;
            MaxViewDistance = data.Range;

            StyleSheet.Load("/map/RoleButton/RoleButtonPoint.scss");

            Hud.Current.RootPanel.AddChild(this);

            Entity = Sandbox.Entity.FindByIndex(Data.NetworkIdent) as TTTRoleButton;

            DescriptionLabel = Add.Label(Entity.Description);
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Vector3 screenPos = Position.ToScreen();
            IsShowing = screenPos.z > 0f;

            //If our entity is locked, delayed or removed, let's not show it.
            if(Entity.IsDisabled)
            {
                //Since we're just technically just making it invisible. Let's go ahead and move it off screen so it doesn't interfere with UI input.
                Style.Left = -10;
                Style.Top = -10;
                Style.Opacity = 0;

                //Make sure our client is no longer tracking this element.
                if (TTTPlayer.FocusedButton == this)
                {
                    TTTPlayer.FocusedButton = null;
                }

                IsShowing = false;
            }

            if (IsShowing)
            {
                Style.Left = Length.Fraction(screenPos.x);
                Style.Top = Length.Fraction(screenPos.y);
                
                Style.Opacity = MathX.Clamp(1.0f - (player.Position.Distance(Position) - MINVIEWDISTANCE) / (MaxViewDistance - MINVIEWDISTANCE), 0.0f, 1.0f);

                //Update our 'focus' CSS look if our player currently is looking near this point.
                if (TTTPlayer.FocusedButton == this)
                {
                    SetClass("focus", true);
                }
                else
                {
                    SetClass("focus", false);
                }

                //Check if point is within 10% of the crosshair.
                if (IsLengthWithinCamerasFocus() && player.Position.Distance(Position) <= MaxViewDistance)
                {
                    TTTPlayer.FocusedButton ??= this; //If the current focused button is null, update it to this.
                }
                else if (TTTPlayer.FocusedButton == this) //If we are the current focused button, but we are out of focus, set to null
                {
                    TTTPlayer.FocusedButton = null;
                }

                Style.Dirty();
            }
        }

        //Our "screen focus" size, roughly %5 of the screen around the cross hair.
        //It might be worth considering using an alternate method to percentages for larger screens. Hoping we can test that with someone who has a UHD monitor.
        private float FocusSize = 2.5f;

        public bool IsLengthWithinCamerasFocus()
        {
            //We have to adjust the top check by the screen's aspect ratio in order to compensate for screen size
            float topHeight = FocusSize * Screen.Aspect;

            //I think we could alternatively use 
            return Style.Left.Value.Value > 50 - FocusSize && Style.Left.Value.Value < 50 + FocusSize
                &&
                Style.Top.Value.Value > 50 - topHeight && Style.Top.Value.Value < 50 + topHeight;
        }

        //Check to make sure player is within range and our button is not disabled.
        //Called when client calls for button to be activated. A simple double check.
        public bool IsUsable(TTTPlayer player)
        {
            return player.Position.Distance(Position) <= Data.Range && !Data.IsDisabled;
        }
    }
}


