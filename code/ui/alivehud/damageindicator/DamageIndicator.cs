using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Events;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class DamageIndicator : Panel
    {
        public static DamageIndicator Instance;

        private float _maxDamageIndicatorDuration = 5f;
        private float _currentRemainingDamageIndicatorDuration = 0f;
        private TimeSince _timeSinceLastDamage = 0f;
        private float _lastDamage = 0f;

        public DamageIndicator()
        {
            Instance = this;

            StyleSheet.Load("/ui/alivehud/damageindicator/DamageIndicator.scss");

            Style.SetBackgroundImage(Texture.Load("/ui/damageindicator/default.png"));
        }

        [Event(TTTEvent.Player.TakeDamage)]
        private void OnTakeDamage(TTTPlayer player, float damage)
        {
            if (Host.IsServer)
            {
                return;
            }

            _lastDamage = damage;
            _timeSinceLastDamage = 0f;
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                Style.Opacity = 0;
                return;
            }

            float remainingDamageIndicatorTime = _lastDamage / player.MaxHealth * 20;

            if (_currentRemainingDamageIndicatorDuration != 0f)
            {
                remainingDamageIndicatorTime += _currentRemainingDamageIndicatorDuration;
                _currentRemainingDamageIndicatorDuration = 0f;
            }

            remainingDamageIndicatorTime = Math.Min(remainingDamageIndicatorTime, _maxDamageIndicatorDuration);

            if (_lastDamage > 0f && _timeSinceLastDamage < remainingDamageIndicatorTime)
            {
                _currentRemainingDamageIndicatorDuration = remainingDamageIndicatorTime - _timeSinceLastDamage;

                Style.Display = DisplayMode.Flex;
                Style.Opacity = Math.Clamp((_currentRemainingDamageIndicatorDuration / remainingDamageIndicatorTime) * (remainingDamageIndicatorTime / _maxDamageIndicatorDuration), 0f, 1f);
            }
            else
            {
                _currentRemainingDamageIndicatorDuration = 0f;

                Style.Display = DisplayMode.None;
            }

            Style.Dirty();
        }
    }
}
