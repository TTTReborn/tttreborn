using System;

using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        private const float FALLING_OBLIGINGNESS = 260f;
        private float _highestZ;
        private Entity _lastGroundEntity;

        public void TickPlayerFalling()
        {
            if (GroundEntity == null)
            {
                if (_highestZ < Position.z)
                {
                    _highestZ = Position.z;
                }

                _lastGroundEntity = null;

                return;
            }

            if (_lastGroundEntity == null)
            {
                OnPlayerHitGround(_highestZ - Position.z);
            }

            _highestZ = Position.z;
            _lastGroundEntity = GroundEntity;
        }

        public void OnPlayerHitGround(float fallingHeight)
        {
            if (fallingHeight > FALLING_OBLIGINGNESS)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    Attacker = this,
                    Body = PhysicsBody,
                    Damage = (float) Math.Round(0.15f * (fallingHeight - FALLING_OBLIGINGNESS)),
                    Flags = DamageFlags.Fall,
                    HitboxIndex = (int) HitboxIndex.LeftFoot,
                    Position = Position
                };

                TakeDamage(damageInfo);
            }
        }
    }
}
