using Sandbox;

namespace TTTReborn.Weapons
{
    [Library("ttt_smg", Title = "MP5")]
    partial class SMG : Weapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
        public override float PrimaryRate => 10.0f;
        public override float SecondaryRate => 1.0f;
        public override int ClipSize => 30;
        public override float ReloadTime => 4.0f;
        public override bool HasFlashlight => true;
        public override bool HasLaserDot => true;
        public override int BaseDamage => 5;
        public override int Bucket => 2;

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_smg/rust_smg.vmdl");
        }

        public override void AttackPrimary()
        {
            if (!TakeAmmo(1))
            {
                PlaySound("pistol.dryfire");

                return;
            }

            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            ShootEffects();
            PlaySound("rust_smg.shoot");
            ShootBullet(0.1f, 1.5f, BaseDamage, 3.0f);
        }

        [ClientRpc]
        protected override void ShootEffects()
        {
            Host.AssertClient();

            Particles.Create("particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle");
            Particles.Create("particles/pistol_ejectbrass.vpcf", EffectEntity, "ejection_point");

            if (IsLocalPawn)
            {
                new Sandbox.ScreenShake.Perlin(0.5f, 4.0f, 1.0f, 0.5f);
            }

            ViewModelEntity?.SetAnimBool("fire", true);
            CrosshairPanel?.OnEvent("fire");
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            anim.SetParam("holdtype", 2);
            anim.SetParam("aimat_weight", 1.0f);
        }
    }

}
