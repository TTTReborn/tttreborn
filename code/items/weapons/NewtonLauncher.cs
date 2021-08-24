using Sandbox;

using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_newton_launcher")]
    [Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
    partial class NewtonLauncher : TTTWeapon, IBuyableItem
    {
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        public override SlotType SlotType => SlotType.Equipment;
        public override bool UnlimitedAmmo => true;
        public override int ClipSize => 1;
        public override float PrimaryRate => 1f;
        public override float SecondaryRate => 1.0f;
        public override float ReloadTime => 2.3f;
        public override float DeployTime => 0.4f;
        public override int BaseDamage => 3;

        public virtual int Price => 100;

        public static int NEWTON_FORCE_MIN => 300;
        public static int NEWTON_FORCE_MAX => 700;
        public static float NEWTON_CHARGE_TIME => 3;

        public bool IsCharging { get; set; } = false;
        public float ChargingStartTime;

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_pistol/rust_pistol.vmdl");
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

        public override void AttackPrimary()
        {
            if (IsCharging) return;

            IsCharging = true;
            ChargingStartTime = Time.Now;
        }

        public void ReleaseCharge()
        {
            (Owner as AnimEntity).SetAnimBool("b_attack", true);
            ShootEffects();
            PlaySound("rust_pistol.shoot").SetPosition(Position).SetVolume(0.8f);
            ShootBullet(0f, 0f, BaseDamage, 0f);
            IsCharging = false;
            TimeSincePrimaryAttack = 0;
        }

        public override void ShootBullet(float spread, float force, float damage, float bulletSize)
        {
            Vector3 forward = Owner.EyeRot.Forward;
            forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * 5000, bulletSize))
            {
                tr.Surface.DoBulletImpact(tr);

                if (!tr.Entity.IsValid() || tr.Entity.IsWorld) return;
                if (!IsServer)
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);

                    Vector3 pushDirection = new Vector3(-tr.Normal.x, -tr.Normal.y, tr.Normal.z < 0.5F ? 0.5F : -0.5F);
                    float chargePercentage = MathX.Clamp((Time.Now - ChargingStartTime) / NEWTON_CHARGE_TIME, 0, 1);
                    float chargeForce = ((NEWTON_FORCE_MAX - NEWTON_FORCE_MIN) * chargePercentage) + NEWTON_FORCE_MIN;

                    tr.Entity.GroundEntity = null;
                    tr.Entity.LocalVelocity = pushDirection * chargeForce;
                }
            }
        }

        public override void CreateHudElements()
        {
            if (Local.Hud == null)
            {
                return;
            }

            // TODO: Give users a way to change their crosshair.
            CrosshairPanel = new Crosshair().SetupCrosshair(new Crosshair.Properties(true,
                false,
                false,
                10,
                2,
                0,
                0,
                3,
                Color.Green));
            CrosshairPanel.Parent = Local.Hud;
            CrosshairPanel.AddClass(ClassInfo.Name);
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
