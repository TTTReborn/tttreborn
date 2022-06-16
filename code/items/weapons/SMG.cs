using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_smg")]
    [Weapon(CarriableCategories.SMG)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_smg/v_rust_smg.vmdl", "weapons/rust_smg/rust_smg.vmdl", "particles/pistol_muzzleflash.vpcf", "particles/pistol_ejectbrass.vpcf")]
    [EditorModel("weapons/rust_smg/rust_smg.vmdl")]
    [Display(Name = "SMG", GroupName = "Weapons")]
    [Title("SMG")]
    public partial class SMG : Weapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
        public override string ModelPath => "weapons/rust_smg/rust_smg.vmdl";

        public override WeaponInfo WeaponInfo { get; set; } = new()
        {
            DeployTime = 0.6f
        };

        public override ClipInfo[] ClipInfos { get; set; } = new ClipInfo[]
        {
            new()
            {
                ClipSize = 30,
                StartAmmo = 30,
                Damage = 8,
                Spread = 0.1f,
                RPM = 600,
                ShootSound = "rust_pistol.shoot",
                Force = 1.5f,
                BulletSize = 3f,
                ShootEffectList = new Dictionary<string, string>()
                {
                    { "particles/pistol_muzzleflash.vpcf", "muzzle" },
                    { "particles/pistol_ejectbrass.vpcf", "ejection_point" }
                },
                ShakeEffect = new()
                {
                    Length = 0.5f,
                    Speed = 4.0f,
                    Size = 1.0f,
                    Rotation = 0.5f
                },
                ReloadTime = 2.8f,
                FiringType = FiringType.AUTO
            }
        };
    }
}
