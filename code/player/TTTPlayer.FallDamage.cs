using System;
using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        public static float FallingObligingness = 260f;
        private float highestZ;
        private Entity lastGroundEntity;

        public void TickPlayerFalling()
        {
            if (GroundEntity == null)
            {
                if (highestZ < Position.z)
                {
                    highestZ = Position.z;
                }

                lastGroundEntity = null;

                return;
            }

            if (lastGroundEntity == null)
            {
                OnPlayerHitGround(highestZ - Position.z);
            }

            highestZ = Position.z;
            lastGroundEntity = GroundEntity;
        }

        public void OnPlayerHitGround(float fallingHeight)
        {
            if (fallingHeight > FallingObligingness)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.Attacker = this;
                damageInfo.Body = PhysicsBody;
                damageInfo.Damage = (float) Math.Round(0.15f * (fallingHeight - FallingObligingness));
                damageInfo.Flags = DamageFlags.Fall;
                damageInfo.HitboxIndex = (int) HitboxIndex.LeftFoot;
                damageInfo.Position = Position;

                TakeDamage(damageInfo);
            }
        }
    }
}
