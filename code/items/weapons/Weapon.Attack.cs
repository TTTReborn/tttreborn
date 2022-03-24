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
                ShootBullet(clipInfo);
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

        public void ShootBullet(ClipInfo clipInfo)
        {
            ShootBullet(clipInfo.Spread, clipInfo.Force, clipInfo.Damage, clipInfo.BulletSize, clipInfo.ImpactEffect, clipInfo.DamageType);
        }

        public virtual void ShootBullet(float spread, float force, float damage, float bulletSize, string impactEffect = null, DamageFlags damageType = DamageFlags.Bullet)
        {
            Vector3 forward = Owner.EyeRotation.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult trace in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * BulletRange, bulletSize))
            {
                if (!IsServer || !trace.Entity.IsValid())
                {
                    continue;
                }

                Vector3 endPos = trace.EndPosition + trace.Direction * bulletSize;

                if (string.IsNullOrEmpty(impactEffect))
                {
                    trace.Surface.DoBulletImpact(trace);
                }
                else
                {
                    Particles.Create(impactEffect, endPos)?.SetForward(0, trace.Normal);
                }

                using (Prediction.Off())
                {
                    DamageInfo damageInfo = new DamageInfo()
                        .WithPosition(trace.EndPosition)
                        .WithFlag(damageType)
                        .WithForce(forward * 100f * force)
                        .UsingTraceResult(trace)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    damageInfo.Damage = damage;

                    trace.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public virtual IEnumerable<TraceResult> TraceBullet(Vector3 start, Vector3 end, float radius = 2.0f)
        {
            bool InWater = Sandbox.Internal.GlobalGameNamespace.Map.Physics.IsPointWater(start);

            yield return Trace.Ray(start, end)
                .UseHitboxes()
                .HitLayer(CollisionLayer.Water, !InWater)
                .HitLayer(CollisionLayer.Debris)
                .Ignore(Owner)
                .Ignore(this)
                .Size(radius)
                .Run();
        }
    }
}
