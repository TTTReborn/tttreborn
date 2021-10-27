using System;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("weapon_smg")]
    [Weapon(SlotType = SlotType.Primary, AmmoType = "smg")]
    [Spawnable]
    [Buyable(Price = 100)]
    [Hammer.EditorModel("weapons/rust_smg/rust_smg.vmdl")]
    partial class SMG : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
        public override float PrimaryRate => 10.0f;
        public override float SecondaryRate => 1.0f;
        public override int ClipSize => 30;
        public override float ReloadTime => 2.8f;
        public override float DeployTime => 0.6f;
        public override int BaseDamage => 8;
        public override Type AmmoEntity => typeof(SMGAmmo);

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_smg/rust_smg.vmdl");
        }

        public override void AttackPrimary()
        {
            if (!TakeAmmo(1))
            {
                PlaySound("pistol.dryfire").SetPosition(Position).SetVolume(0.2f);

                return;
            }

            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            ShootEffects();

            PlaySound("rust_smg.shoot").SetPosition(Position).SetVolume(0.8f);
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
            CrosshairPanel?.CreateEvent("fire");
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            anim.SetParam("holdtype", 2);
            anim.SetParam("aimat_weight", 1.0f);
        }
    }
}
