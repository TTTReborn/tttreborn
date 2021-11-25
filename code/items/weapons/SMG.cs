// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("weapon_smg")]
    [Weapon(SlotType = SlotType.Primary, AmmoType = "ammo_smg")]
    [Spawnable]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_smg/v_rust_smg.vmdl", "weapons/rust_smg/rust_smg.vmdl", "particles/pistol_muzzleflash.vpcf", "particles/pistol_ejectbrass.vpcf")]
    [Hammer.EditorModel("weapons/rust_smg/rust_smg.vmdl")]
    partial class SMG : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_smg/v_rust_smg.vmdl";
        public override string ModelPath => "weapons/rust_smg/rust_smg.vmdl";
        public override float PrimaryRate => 10.0f;
        public override float SecondaryRate => 1.0f;
        public override int ClipSize => 30;
        public override float ReloadTime => 2.8f;
        public override float DeployTime => 0.6f;
        public override int BaseDamage => 8;
        public override Type AmmoEntity => typeof(SMGAmmo);

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
