using Sandbox;

namespace TTTReborn.Weapons
{
    [Library("ttt_pistol", Title = "Baretta")]
    partial class Pistol : Weapon
    {
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

        public override bool UnlimitedAmmo => true;
        public override int ClipSize => 10;
        public override float PrimaryRate => 15.0f;
        public override float SecondaryRate => 1.0f;
        public override float ReloadTime => 3.0f;
        public override bool HasLaserDot => true;
        public override int BaseDamage => 8;
        public override int Bucket => 1;

        public override void Spawn()
        {
            base.Spawn();

            SetModel("weapons/rust_pistol/rust_pistol.vmdl");
        }

        public override bool CanPrimaryAttack()
        {
            return base.CanPrimaryAttack() && Owner.Input.Pressed(InputButton.Attack1);
        }

        public override void AttackPrimary()
        {
            if (!TakeAmmo(1))
            {
                PlaySound("pistol.dryfire");

                return;
            }

            ShootEffects();
            PlaySound("rust_pistol.shoot");
            ShootBullet(0.05f, 1.5f, BaseDamage, 3.0f);
        }
    }

}
