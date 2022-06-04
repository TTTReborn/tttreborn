using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_shotgun")]
    [Weapon(CarriableCategories.Shotgun)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl", "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl", "particles/pistol_muzzleflash.vpcf", "particles/pistol_ejectbrass.vpcf")]
    [EditorModel("weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl")]
    [Display(Name = "Shotgun", GroupName = "Weapons")]
    [Title("Shotgun")]
    public partial class Shotgun : Weapon
    {
        public override string ViewModelPath => "weapons/rust_pumpshotgun/v_rust_pumpshotgun.vmdl";
        public override string ModelPath => "weapons/rust_pumpshotgun/rust_pumpshotgun.vmdl";

        public override WeaponInfo WeaponInfo { get; set; } = new()
        {
            DeployTime = 0.6f
        };

        public override ClipInfo[] ClipInfos { get; set; } = new ClipInfo[]
        {
            new()
            {
                ClipSize = 8,
                StartAmmo = 8,
                Damage = 6,
                Bullets = 10,
                RPM = 60,
                ShootSound = "rust_pumpshotgun.shoot",
                DryFireSound = "pistol.dryfire",
                ShootEffectList = new Dictionary<string, string>()
                {
                    { "particles/pistol_muzzleflash.vpcf", "muzzle" },
                    { "particles/pistol_ejectbrass.vpcf", "ejection_point" }
                },
                ShakeEffect = new()
                {
                    Length = 1.0f,
                    Speed = 1.5f,
                    Size = 2.0f
                },
                BulletsPerReload = 1,
                ReloadTime = 0.5f,
                IsPartialReloading = true,
                FiringType = FiringType.SEMI
            }
        };
    }
}
