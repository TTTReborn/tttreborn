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

        public override WeaponInfo WeaponInfo { get; set; } = new()
        {
            DeployTime = 0.4f
        };

        public override ClipInfo Primary { get; set; } = new()
        {
            UnlimitedAmmo = true,
            ClipSize = 15,
            StartAmmo = 15,
            Damage = 8,
            RPM = 180,
            ShootSound = "rust_pistol.shoot",
            DryFireSound = "pistol.dryfire",
            Spread = 0.05f,
            Force = 1.5f,
            BulletSize = 0.1f,
            ReloadTime = 2.3f
        };
    }
}
