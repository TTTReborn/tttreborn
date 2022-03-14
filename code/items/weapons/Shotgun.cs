using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_shotgun")]
    [Weapon(CarriableCategories.Shotgun)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl", "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl", "particles/pistol_muzzleflash.vpcf", "particles/pistol_ejectbrass.vpcf")]
    [Hammer.EditorModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl")]
    public partial class Shotgun : ShotgunWeapon
    {
        public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
        public override string ModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

        public Shotgun() : base()
        {
            WeaponInfo.ReloadTime = 0.5f;
            WeaponInfo.DeployTime = 0.6f;

            Primary.ClipSize = 8;
            Primary.Damage = 6;
            Primary.Bullets = 10;
            Primary.RPM = 60;
            Primary.ShootSound = "rust_pumpshotgun.shoot";
            Primary.DryFireSound = "pistol.dryfire";
            Primary.ShootEffectList = new Dictionary<string, string>()
            {
                { "particles/pistol_muzzleflash.vpcf", "muzzle" },
                { "particles/pistol_ejectbrass.vpcf", "ejection_point" }
            };
            Primary.ShakeEffect = new()
            {
                Length = 1.0f,
                Speed = 1.5f,
                Size = 2.0f
            };
        }
    }
}
