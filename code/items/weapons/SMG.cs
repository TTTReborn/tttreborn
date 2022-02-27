using Sandbox;

namespace TTTReborn.Items
{
    [Library("weapon_smg")]
    [Weapon(CarriableCategories.SMG)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_smg/v_rust_smg.vmdl", "weapons/rust_smg/rust_smg.vmdl", "particles/pistol_muzzleflash.vpcf", "particles/pistol_ejectbrass.vpcf")]
    [Hammer.EditorModel("weapons/rust_smg/rust_smg.vmdl")]
    public partial class SMG : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
        public override string ModelPath => "weapons/rust_smg/rust_smg.vmdl";
        public override float PrimaryRate => 10.0f;
        public override float SecondaryRate => 1.0f;
        public override int ClipSize => 30;
        public override float ReloadTime => 2.8f;
        public override float DeployTime => 0.6f;
        public override int BaseDamage => 8;

        public override void AttackPrimary()
        {
            if (!TakeAmmo(1))
            {
                PlaySound("pistol.dryfire").SetPosition(Position).SetVolume(0.2f);

                return;
            }

            (Owner as AnimEntity).SetAnimParameter("b_attack", true);

            if (IsClient)
            {
                ShootEffects();
            }

            PlaySound("rust_smg.shoot").SetPosition(Position).SetVolume(0.8f);
            ShootBullet(0.1f, 1.5f, BaseDamage, 3.0f);
        }

        protected override void ShootEffects()
        {
            Host.AssertClient();

            Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

            if (IsLocalPawn)
            {
                using (Prediction.Off())
                {
                    _ = new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
                }
            }

            ViewModelEntity?.SetAnimParameter("fire", true);
            CrosshairPanel?.CreateEvent("fire");
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            anim.SetAnimParameter("holdtype", 2);
            anim.SetAnimParameter("aim_body_weight", 1.0f);
        }
    }
}
