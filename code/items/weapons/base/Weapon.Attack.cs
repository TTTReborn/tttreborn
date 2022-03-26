using System.Collections.Generic;

using Sandbox;
using Sandbox.ScreenShake;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public virtual bool CanAttack(ClipInfo clipInfo, InputButton inputButton)
        {
            if (clipInfo == null || !Owner.IsValid() || !Input.Down(inputButton) || TimeSinceDeployed <= WeaponInfo.DeployTime || !clipInfo.IsPartialReloading && IsReloading)
            {
                return false;
            }

            if (clipInfo.RPM <= 0)
            {
                return true;
            }

            return (clipInfo == Primary ? TimeSincePrimaryAttack : TimeSinceSecondaryAttack) > GetRealRPM(clipInfo.RPM);
        }

        public virtual void Attack(ClipInfo clipInfo)
        {
            if (clipInfo == null)
            {
                return;
            }

            bool isPrimary = clipInfo == Primary;

            if (clipInfo.IsPartialReloading && isPrimary ? IsPrimaryReloading : IsSecondaryReloading)
            {
                OnReloadFinish(clipInfo);
            }

            if (isPrimary)
            {
                TimeSincePrimaryAttack = 0f;
            }
            else
            {
                TimeSinceSecondaryAttack = 0f;
            }

            if (!TakeAmmo(clipInfo, 1))
            {
                PlaySound(clipInfo.DryFireSound).SetPosition(Position).SetVolume(0.2f);

                return;
            }

            (Owner as AnimEntity).SetAnimParameter("b_attack", true);

            PlaySound(clipInfo.ShootSound).SetPosition(Position).SetVolume(0.8f);

            Rand.SetSeed(Time.Tick);

            ShootEffects(GetClipInfoIndex(clipInfo));

            for (int i = 0; i < clipInfo.Bullets; i++)
            {
                ShootBullet(clipInfo);
            }
        }

        [ClientRpc]
        public virtual void ShootEffects(int clipInfoIndex)
        {
            Host.AssertClient();

            ClipInfo clipInfo = GetClipInfoByIndex(clipInfoIndex);

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
                _ = new Perlin(clipInfo.ShakeEffect.Length, clipInfo.ShakeEffect.Speed, clipInfo.ShakeEffect.Size, clipInfo.ShakeEffect.Rotation);
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
                Vector3 endPos = trace.EndPosition + trace.Direction * bulletSize;

                if (string.IsNullOrEmpty(impactEffect))
                {
                    trace.Surface.DoBulletImpact(trace);
                }
                else
                {
                    Particles.Create(impactEffect, endPos)?.SetForward(0, trace.Normal);
                }

                if (!IsServer || !trace.Entity.IsValid())
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    DealDamage(trace.Entity, trace.EndPosition, forward * 100f * force, damage, damageType, trace);
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

        public virtual void DealDamage(Entity target, Vector3 position, Vector3 force, float damage, DamageFlags damageType, TraceResult? traceResult = null)
        {
            DamageInfo damageInfo = new DamageInfo()
                .WithPosition(position)
                .WithFlag(damageType)
                .WithForce(force)
                .WithAttacker(Owner)
                .WithWeapon(this);

            if (traceResult != null)
            {
                damageInfo.UsingTraceResult((TraceResult) traceResult);
            }

            damageInfo.Damage = damage;

            target.TakeDamage(damageInfo);
        }
    }
}
