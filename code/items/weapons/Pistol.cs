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

using Sandbox;

namespace TTTReborn.Items
{
    [Library("weapon_pistol")]
    [Weapon(SlotType = SlotType.Secondary, AmmoType = "ammo_pistol")]
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

            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            ShootEffects();

            PlaySound("rust_pistol.shoot").SetPosition(Position).SetVolume(0.8f);
            ShootBullet(0.05f, 1.5f, BaseDamage, 3.0f);
        }
    }
}
