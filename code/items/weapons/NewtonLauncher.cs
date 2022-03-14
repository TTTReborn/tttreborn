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

        public NewtonLauncher() : base()
        {
            WeaponInfo.ReloadTime = 2.3f;
            WeaponInfo.DeployTime = 0.4f;

            Primary.UnlimitedAmmo = true;
            Primary.ClipSize = 1;
            Primary.Damage = 3;
            Primary.RPM = 60;
            Primary.ShootSound = "rust_pistol.shoot";
            Primary.DryFireSound = "pistol.dryfire";
        }

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
                ShootEffects(Primary);
            }

            PlaySound(Primary.ShootSound).SetPosition(Position).SetVolume(0.8f);
            ShootBullet(0f, 0f, Primary.Damage, 0f);

            IsCharging = false;
        }

        public override void ShootBullet(float spread, float force, float damage, float bulletSize)
        {
            Vector3 forward = Owner.EyeRotation.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePosition, Owner.EyePosition + forward * 5000, bulletSize))
            {
                if (!IsServer || !tr.Entity.IsValid() || tr.Entity.IsWorld)
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPosition, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Surface.DoBulletImpact(tr);
                    tr.Entity.TakeDamage(damageInfo);

                    Vector3 pushDirection = new(tr.Direction.x, tr.Direction.y, Math.Min(tr.Direction.z + 0.5F, 0.75F));
                    float chargePercentage = Math.Clamp((Time.Now - ChargingStartTime) / NEWTON_CHARGE_TIME, 0, 1);
                    float chargeForce = ((NEWTON_FORCE_MAX - NEWTON_FORCE_MIN) * chargePercentage) + NEWTON_FORCE_MIN;

                    tr.Entity.GroundEntity = null;
                    tr.Entity.ApplyAbsoluteImpulse(pushDirection * chargeForce);
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
