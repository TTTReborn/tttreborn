using Sandbox;

using TTTReborn.Player.Camera;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private const float MAX_HINT_DISTANCE = 2048;

        private EntityHintPanel _currentHintPanel;
        private IEntityHint _currentHint;

        private void TickEntityHints()
        {
            if (Local.Pawn is not TTTPlayer player || player.Camera is ThirdPersonSpectateCamera)
            {
                DeleteHint();

                return;
            }

            IEntityHint hint = player.IsLookingAtHintableEntity(MAX_HINT_DISTANCE);

            if (hint != null && _currentHintPanel != null)
            {
                _currentHintPanel.UpdateHintPanel();

                if (!hint.CanHint(player) || hint != _currentHint)
                {
                    DeleteHint();

                    return;
                }
            }

            // If we are looking at a hint and don't have a current hint, let's see if we can make one.
            if (hint != null)
            {
                if (hint.CanHint(player) && _currentHintPanel == null)
                {
                    _currentHintPanel = hint.DisplayHint(player);
                    _currentHintPanel.Parent = HintDisplay.Instance;
                    _currentHintPanel.Enabled = true;
                    _currentHintPanel.UpdateHintPanel();

                    _currentHint = hint;
                }
            }
            else
            {
                // If we just looked away, disable and update the panel
                if (_currentHintPanel != null)
                {
                    _currentHintPanel.Enabled = false;
                    _currentHintPanel.UpdateHintPanel();
                }

                DeleteHint();
            }
        }

        private void DeleteHint()
        {
            _currentHintPanel?.Delete();
            _currentHintPanel = null;
            _currentHint = null;
        }
    }
}
