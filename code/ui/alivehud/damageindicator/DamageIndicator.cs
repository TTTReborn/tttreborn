using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class DamageIndicator : TTTPanel
    {
        public static DamageIndicator Instance;

        private float _maxDamageIndicatorDuration = 10f;
        private float _currentRemainingDamageIndicatorDuration = 0f;
        private TimeSince _timeSinceLastDamage = 0f;
        private float _lastDamage = 0f;
        private float _additionalDamageIndicatorDuration = 0f;

        public DamageIndicator()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/damageindicator/DamageIndicator.scss");

            Style.SetBackgroundImage(Texture.Load("/ui/damageindicator/default.png"));
        }

        [Event("tttreborn.player.takedamage")]
        private void OnTakeDamage(TTTPlayer player, float damage)
        {
            if (Host.IsServer)
            {
                return;
            }

            _lastDamage = damage;
            _timeSinceLastDamage = 0f;
            _additionalDamageIndicatorDuration += _currentRemainingDamageIndicatorDuration;
            _currentRemainingDamageIndicatorDuration = 0f;
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            float remainingDamageIndicatorTime = Math.Max(_lastDamage / player.MaxHealth, _maxDamageIndicatorDuration);

            if (_additionalDamageIndicatorDuration != 0f)
            {
                remainingDamageIndicatorTime += _additionalDamageIndicatorDuration;
                _additionalDamageIndicatorDuration = 0f;
            }

            if (_lastDamage > 0f && _timeSinceLastDamage < remainingDamageIndicatorTime)
            {
                _currentRemainingDamageIndicatorDuration = remainingDamageIndicatorTime - _timeSinceLastDamage;

                Style.Display = DisplayMode.Flex;
                Style.Opacity = Math.Clamp((_currentRemainingDamageIndicatorDuration / remainingDamageIndicatorTime) * (remainingDamageIndicatorTime / _maxDamageIndicatorDuration), 0f, 1f);
                Style.Dirty();
            }
            else
            {
                _currentRemainingDamageIndicatorDuration = 0f;

                Style.Display = DisplayMode.None;
            }
        }
    }
}
