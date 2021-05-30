using Sandbox;

namespace TTTGamemode
{
    [ClassLibrary("ttt_smg", Title = "SMG")]
    partial class SMG : Weapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";

        public override float PrimaryRate => 15.0f;
        public override float SecondaryRate => 1.0f;
        public override int ClipSize => 30;
        public override float ReloadTime => 4.0f;
        public override bool HasFlashlight => true;
        public override int Bucket => 2;

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_smg/rust_smg.vmdl");
        }

        public override void AttackPrimary(Sandbox.Player owner)
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

            if (!TakeAmmo(1))
            {
                PlaySound("pistol.dryfire");

                return;
            }

            Owner.SetAnimParam("b_attack", true);

            ShootEffects();
            PlaySound("rust_smg.shoot");
            ShootBullet(0.1f, 1.5f, 5.0f, 3.0f);
        }

        [ClientRpc]
        protected override void ShootEffects()
        {
            Host.AssertClient();

            Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

            if (Owner == TTTPlayer.Local)
            {
                _ = new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
            }

            ViewModelEntity?.SetAnimParam("fire", true);
            CrosshairPanel?.OnEvent("fire");
        }

        public override void TickPlayerAnimator(PlayerAnimator anim)
        {
            anim.SetParam("holdtype", 2);
            anim.SetParam("aimat_weight", 1.0f);
        }
    }
}
