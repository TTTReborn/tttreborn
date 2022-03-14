using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_pistol")]
    [Weapon(CarriableCategories.Pistol)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_pistol/v_rust_pistol.vmdl", "weapons/rust_pistol/rust_pistol.vmdl")]
    [Hammer.EditorModel("weapons/rust_pistol/rust_pistol.vmdl")]
    public partial class Pistol : Weapon
    {
        public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";
        public override string ModelPath => "weapons/rust_pistol/rust_pistol.vmdl";

        public Pistol() : base()
        {
            WeaponInfo.ReloadTime = 2.3f;
            WeaponInfo.DeployTime = 0.4f;

            Primary.UnlimitedAmmo = true;
            Primary.ClipSize = 15;
            Primary.Damage = 8;
            Primary.Rate = 15f;
            Primary.ShootSound = "rust_pistol.shoot";
            Primary.DryFireSound = "pistol.dryfire";
            Primary.Spread = 0.05f;
            Primary.Force = 1.5f;
            Primary.BulletSize = 0.1f;
        }
    }
}
