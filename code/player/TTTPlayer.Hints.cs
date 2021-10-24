using Sandbox;

using TTTReborn.Hints;
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

            if (hint == null || !hint.CanHint(player))
            {
                DeleteHint();

                return;
            }

            if (hint == _currentHint)
            {
                _currentHintPanel.UpdateHintPanel(hint);

                return;
            }

            DeleteHint();

            _currentHintPanel = hint.DisplayHintPanel(player);
            _currentHintPanel.Parent = HintDisplay.Instance;
            _currentHintPanel.Enabled = true;

            _currentHint = hint;
        }

        private void DeleteHint()
        {
            _currentHintPanel?.Delete(true);
            _currentHintPanel = null;
            _currentHint = null;
        }
    }
}
