using Sandbox;
using System;

namespace TTTGamemode
{
    [ClassLibrary("ttt_pistol", Title = "Pistol")]
    partial class Pistol : Weapon
    {
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

        public override bool UnlimitedAmmo => true;
        public override int ClipSize => 10;
        public override float PrimaryRate => 15.0f;
        public override float SecondaryRate => 1.0f;
        public override float ReloadTime => 3.0f;
        public override int Bucket => 1;

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_pistol/rust_pistol.vmdl");
        }

        public override bool CanPrimaryAttack(Sandbox.Player owner)
        {
            return base.CanPrimaryAttack(owner) && owner.Input.Pressed(InputButton.Attack1);
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

            ShootEffects();
            PlaySound("rust_pistol.shoot");
            ShootBullet(0.05f, 1.5f, 9.0f, 3.0f);
        }
    }
}
