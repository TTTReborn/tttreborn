using Sandbox;

namespace TTTReborn.Items
{
    [Library("weapon_pistol")]
    [Weapon(CarriableCategories.Pistol)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_pistol/v_rust_pistol.vmdl", "weapons/rust_pistol/rust_pistol.vmdl")]
    [Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
    public partial class Pistol : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        public override string ModelPath => "weapons/rust_pistol/rust_pistol.vmdl";
        public override bool UnlimitedAmmo => true;
        public override int ClipSize => 15;
        public override float PrimaryRate => 15.0f;
        public override float SecondaryRate => 1.0f;
        public override float ReloadTime => 2.3f;
        public override float DeployTime => 0.4f;
        public override int BaseDamage => 8;

        public override bool CanPrimaryAttack()
        {
            return base.CanPrimaryAttack() && Input.Pressed(InputButton.Attack1);
        }

        public override void AttackPrimary()
        {
            TimeSincePrimaryAttack = 0;
            TimeSinceSecondaryAttack = 0;

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

            PlaySound("rust_pistol.shoot").SetPosition(Position).SetVolume(0.8f);
            ShootBullet(0.05f, 1.5f, BaseDamage, 3.0f);
        }
    }
}
