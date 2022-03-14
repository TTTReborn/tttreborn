using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_weapon_smg")]
    [Weapon(CarriableCategories.SMG)]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_smg/v_rust_smg.vmdl", "weapons/rust_smg/rust_smg.vmdl", "particles/pistol_muzzleflash.vpcf", "particles/pistol_ejectbrass.vpcf")]
    [Hammer.EditorModel("weapons/rust_smg/rust_smg.vmdl")]
    public partial class SMG : Weapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
        public override string ModelPath => "weapons/rust_smg/rust_smg.vmdl";

        public SMG() : base()
        {
            WeaponInfo.ReloadTime = 2.8f;
            WeaponInfo.DeployTime = 0.6f;

            Primary.ClipSize = 30;
            Primary.Damage = 8;
            Primary.Spread = 0.1f;
            Primary.RPM = 600;
            Primary.Force = 1.5f;
            Primary.BulletSize = 3f;
            Primary.ShootEffectList = new()
            {
                new("particles/pistol_muzzleflash.vpcf", "muzzle"),
                new("particles/pistol_ejectbrass.vpcf", "ejection_point")
            };
            Primary.ShakeEffect = new(0.5f, 4.0f, 1.0f, 0.5f);
        }
    }
}
