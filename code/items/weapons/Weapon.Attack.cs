using System.Collections.Generic;

using Sandbox;
using Sandbox.ScreenShake;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public virtual bool CanAttack(ClipInfo clipInfo, InputButton inputButton)
		{
            if (clipInfo == null || !Owner.IsValid() || !Input.Down(inputButton) || TimeSinceDeployed <= WeaponInfo.DeployTime)
            {
                return false;
            }

            if (clipInfo.RPM <= 0)
            {
                return true;
            }

            return clipInfo.TimeSinceAttack > GetRealRPM(clipInfo.RPM);
		}

        public virtual void Attack(ClipInfo clipInfo)
        {
            if (clipInfo == null)
            {
                return;
            }

            clipInfo.TimeSinceAttack = 0f;

            if (!TakeAmmo(clipInfo, 1))
            {
                PlaySound(clipInfo.DryFireSound).SetPosition(Position).SetVolume(0.2f);

                return;
            }

            (Owner as AnimEntity).SetAnimParameter("b_attack", true);

            if (IsClient)
            {
                ShootEffects(clipInfo);
            }

            PlaySound(clipInfo.ShootSound).SetPosition(Position).SetVolume(0.8f);

            for (int i = 0; i < clipInfo.Bullets; i++)
            {
                ShootBullet(clipInfo.Spread, clipInfo.Force, clipInfo.Damage, clipInfo.BulletSize);
            }
        }

        protected virtual void ShootEffects(ClipInfo clipInfo)
        {
            Host.AssertClient();

            if (clipInfo == null)
            {
                return;
            }

            if (WeaponInfo.Category != CarriableCategories.Melee)
            {
                foreach (KeyValuePair<string, string> keyValuePair in clipInfo.ShootEffectList)
                {
                    Particles.Create(keyValuePair.Key, EffectEntity, keyValuePair.Value);
                }
            }

            if (IsLocalPawn && clipInfo.ShakeEffect != null)
            {
                using (Prediction.Off())
                {
                    _ = new Perlin(clipInfo.ShakeEffect.Length, clipInfo.ShakeEffect.Speed, clipInfo.ShakeEffect.Size, clipInfo.ShakeEffect.Rotation);
                }
            }

            ViewModelEntity?.SetAnimParameter("fire", true);
            CrosshairPanel?.CreateEvent("fire");
        }

        public virtual void ShootBullet(float spread, float force, float damage, float bulletSize)
        {
            Vector3 forward = Owner.EyeRotation.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * 5000, bulletSize))
            {
                if (!IsServer || !tr.Entity.IsValid())
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    tr.Surface.DoBulletImpact(tr);

                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPosition, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public virtual IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float radius = 2.0f)
        {
            using (LagCompensation())
            {
                bool InWater = Sandbox.Internal.GlobalGameNamespace.Map.Physics.IsPointWater(start);

                TraceResult tr = Trace.Ray(start, end)
                    .UseHitboxes()
                    .HitLayer(CollisionLayer.Water, !InWater)
                    .HitLayer(CollisionLayer.Debris)
                    .Ignore(Owner)
                    .Ignore(this)
                    .Size(radius)
                    .Run();

                yield return tr;
            }
        }
    }
}
