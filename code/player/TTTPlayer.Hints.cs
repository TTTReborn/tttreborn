using Sandbox;

using TTTReborn.Player.Camera;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private const float MAX_HINT_DISTANCE = 2048;

        private EntityHintPanel _currentHint;

        private void TickEntityHints()
        {
            if (Local.Pawn is not TTTPlayer player || player.Camera is ThirdPersonSpectateCamera)
            {
                DeleteHint();
                return;
            }

            IEntityHint target = player.IsLookingAtType<IEntityHint>(MAX_HINT_DISTANCE);

            if (target != null && _currentHint != null)
            {
                _currentHint.UpdateHintPanel();

                if (!target.CanHint(player))
                {
                    DeleteHint();
                    return;
                }
            }

            // If we are looking at a target and don't have a current hint, let's see if we can make one.
            if (target != null)
            {
                if (target.CanHint(player) && _currentHint == null)
                {
                    _currentHint = target.DisplayHint(player); 
                    _currentHint.Parent = Hud.Current.RootPanel;
                    _currentHint.Enabled = true;
                    _currentHint.UpdateHintPanel();
                }
            }
            else
            {
                // If we just looked away, disable and update the panel
                if (_currentHint != null)
                {
                    _currentHint.Enabled = false;
                    _currentHint.UpdateHintPanel();
                }

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
