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
    [Library("weapon_knife")]
    [Weapon(SlotType = SlotType.Melee)]
    [Buyable(Price = 100)]
    [Precached("weapons/rust_boneknife/v_rust_boneknife.vmdl", "weapons/rust_boneknife/rust_boneknife.vmdl")]
    [Hammer.EditorModel("weapons/rust_boneknife/rust_boneknife.vmdl")]
    partial class Knife : TTTWeapon
    {
        public override string ViewModelPath => "weapons/rust_boneknife/v_rust_boneknife.vmdl";
        public override string ModelPath => "weapons/rust_boneknife/rust_boneknife.vmdl";
        public override float PrimaryRate => 1.0f;
        public override float DeployTime => 0.2f;
        public override int BaseDamage => 45;
        public virtual int MeleeDistance => 80;

        public virtual void MeleeStrike(float damage, float force)
        {
            Vector3 forward = Owner.EyeRot.Forward;
            forward = forward.Normal;

            foreach (TraceResult tr in TraceBullet(Owner.EyePos, Owner.EyePos + forward * MeleeDistance, 10f))
            {
                if (!tr.Entity.IsValid())
                {
                    continue;
                }

                tr.Surface.DoBulletImpact(tr);

                if (!IsServer)
                {
                    continue;
                }

                using (Prediction.Off())
                {
                    DamageInfo damageInfo = DamageInfo.FromBullet(tr.EndPos, forward * 100 * force, damage)
                        .UsingTraceResult(tr)
                        .WithAttacker(Owner)
                        .WithWeapon(this);

                    tr.Entity.TakeDamage(damageInfo);
                }
            }
        }

        public override void AttackPrimary()
        {
            (Owner as AnimEntity).SetAnimBool("b_attack", true);

            ShootEffects();

            PlaySound("rust_boneknife.attack");
            MeleeStrike(BaseDamage, 1.5f);
        }
    }
}
