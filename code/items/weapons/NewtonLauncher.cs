using System;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_newtonlauncher")]
    [Weapon(CarriableCategories.OffensiveEquipment)]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_pistol/v_rust_pistol.vmdl", "weapons/rust_pistol/rust_pistol.vmdl")]
    [Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
    public partial class NewtonLauncher : Weapon
    {
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        public override string ModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

        public const int NEWTON_FORCE_MIN = 300;
        public const int NEWTON_FORCE_MAX = 700;
        public const float NEWTON_CHARGE_TIME = 3;

        public bool IsCharging { get; set; } = false;
        public float ChargingStartTime;

        public override WeaponInfo WeaponInfo { get; set; } = new()
        {
            DeployTime = 0.4f
        };

        public override ClipInfo[] ClipInfos { get; set; } = new ClipInfo[]
        {
            new()
            {
                UnlimitedAmmo = true,
                ClipSize = 1,
                StartAmmo = 1,
                Damage = 3,
                RPM = 60,
                ShootSound = "rust_pistol.shoot",
                DryFireSound = "pistol.dryfire",
                ReloadTime = 2.3f
            }
        };

        public override void OnActive()
        {
            base.OnActive();

            IsCharging = false;
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            base.ActiveEnd(ent, dropped);

            IsCharging = false;
        }

        public override void Attack(ClipInfo clipInfo)
        {
            if (IsCharging || clipInfo != Primary)
            {
                return;
            }

            IsCharging = true;
            ChargingStartTime = Time.Now;
        }

        public void ReleaseCharge()
        {
            (Owner as AnimEntity).SetAnimParameter("b_attack", true);

            if (IsClient)
            {
                ShootEffects(GetClipInfoIndex(Primary));
            }

            PlaySound(Primary.ShootSound).SetPosition(Position).SetVolume(0.8f);
            ShootBullet(0f, 0f, Primary.Damage, 0f);

            IsCharging = false;
        }

        public override void ShootBullet(float spread, float force, float damage, float bulletSize, string impactEffect = null, DamageFlags damageType = DamageFlags.Crush)
        {
            Vector3 forward = Owner.EyeRotation.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult trace in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * 5000, bulletSize))
            {
                if (!IsServer || !trace.Entity.IsValid() || trace.Entity.IsWorld)
                {
                    continue;
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

                    trace.Surface.DoBulletImpact(trace);
                    trace.Entity.TakeDamage(damageInfo);

                    Vector3 pushDirection = new(trace.Direction.x, trace.Direction.y, Math.Min(trace.Direction.z + 0.5f, 0.75f));
                    float chargePercentage = Math.Clamp((Time.Now - ChargingStartTime) / NEWTON_CHARGE_TIME, 0, 1);
                    float chargeForce = ((NEWTON_FORCE_MAX - NEWTON_FORCE_MIN) * chargePercentage) + NEWTON_FORCE_MIN;

                    trace.Entity.GroundEntity = null;
                    trace.Entity.ApplyAbsoluteImpulse(pushDirection * chargeForce);
                }
            }
        }

        public override void Simulate(Client owner)
        {
            base.Simulate(owner);

            if (IsCharging)
            {
                if (!Input.Down(InputButton.Attack1))
                {
                    ReleaseCharge();
                }
            }
        }
    }
}
