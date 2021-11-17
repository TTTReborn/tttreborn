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
            if (Camera is ThirdPersonSpectateCamera)
            {
                DeleteHint();

                return;
            }

            IEntityHint hint = IsLookingAtHintableEntity(MAX_HINT_DISTANCE);

            if (hint == null)
            {
                DeleteHint();
                return;
            }

            hint?.TickUse(this);

            if (!hint.CanHint(this))
            {
                DeleteHint();
                return;
            }

            if (hint == _currentHint)
            {
                if (IsClient)
                {
                    _currentHintPanel.UpdateHintPanel(hint.TextOnTick);
                }

                return;
            }

            DeleteHint();

            if (IsClient)
            {
                if (hint.ShowGlow && hint is ModelEntity model)
                {
                    model.GlowColor = Color.Blue;
                    model.GlowActive = true;
                }
                _currentHintPanel = hint.DisplayHint(this);
                _currentHintPanel.Parent = HintDisplay.Instance;
                _currentHintPanel.Enabled = true;
            }

            _currentHint = hint;
        }

        private void DeleteHint()
        {
            if (IsClient)
            {
                if (_currentHint is ModelEntity model)
                {
                    model.GlowActive = false;
                }
                _currentHintPanel?.Delete(true);
                _currentHintPanel = null;
            }
            _currentHint?.StopUsing(this);
            _currentHint = null;
        }
    }
}
