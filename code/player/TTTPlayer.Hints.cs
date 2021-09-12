using Sandbox;

using TTTReborn.Player.Camera;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private const float MAX_HINT_DISTANCE = 100;

        private TTTPanel _currentHint;

        private void TickEntityHints()
        {
            if (Local.Pawn is not TTTPlayer player || player.Camera is ThirdPersonSpectateCamera)
            {
                DeleteHint();
                return;
            }

            IEntityHint target = player.IsLookingAtType<IEntityHint>(MAX_HINT_DISTANCE);

            //If we're looking at a valid target and we currently have a hint displayed, double check that the entity still wants us to hint.
            if (target != null && _currentHint != null)
            {
                if (!target.CanHint(player))
                {
                    DeleteHint();
                    return;
                }
            }

            //If we are looking at a target and don't have a current hint, let's see if we can make one.
            if (target != null)
            {
                if (target.CanHint(player) && _currentHint == null)
                {
                    _currentHint = target.DisplayHint(player); //Retrieves panel information from entity
                    _currentHint.Parent = Hud.Current.RootPanel;
                    _currentHint.IsShowing = true;
                }
            }
            else
            {
                //If no target, make sure we don't have a hint anymore.
                DeleteHint();
            }
        }

        private void DeleteHint()
        {
            _currentHint?.Delete();
            _currentHint = null;
        }
    }
}
