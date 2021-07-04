using System;

using Sandbox;

namespace TTTReborn.Player
{
    public enum HitboxIndex
    {
        Pelvis = 1,
        Stomach = 2,
        Rips = 3,
        Neck = 4,
        Head = 5,
        LeftUpperArm = 7,
        LeftLowerArm = 8,
        LeftHand = 9,
        RightUpperArm = 11,
        RightLowerArm = 12,
        RightHand = 13,
        RightUpperLeg = 14,
        RightLowerLeg = 15,
        RightFoot = 16,
        LeftUpperLeg = 17,
        LeftLowerLeg = 18,
        LeftFoot = 19,
    }

    public enum HitboxGroup
    {
        None = -1,
        Generic = 0,
        Head = 1,
        Chest = 2,
        Stomach = 3,
        LeftArm = 4,
        RightArm = 5,
        LeftLeg = 6,
        RightLeg = 7,
        Gear = 10,
        Special = 11,
    }

    public partial class TTTPlayer
    {
        public float MaxHealth { get; set; } = 100f;

        public void SetHealth(float health)
        {
            Health = Math.Min(health, MaxHealth);
        }

        public override void TakeDamage(DamageInfo info)
        {
            if (GetHitboxGroup(info.HitboxIndex) == (int) HitboxGroup.Head)
            {
                info.Damage *= 2.0f;
            }

            if (info.Attacker is TTTPlayer attacker && attacker != this)
            {
                if (Gamemode.Game.Instance.Round is not (Rounds.InProgressRound or Rounds.PostRound))
                {
                    return;
                }

                ClientDidDamage(info.Position, info.Damage, ((float) Health).LerpInverse(100, 0));
            }

            if (info.Weapon != null)
            {
                ClientTookDamage(info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.Position);
            }

            // Play pain sounds
            if ((info.Flags & DamageFlags.Fall) == DamageFlags.Fall)
            {
                PlaySound("fall").SetVolume(0.5f).SetPosition(info.Position);
            }
            else if ((info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet)
            {
                PlaySound("grunt" + Rand.Int(1, 4)).SetVolume(0.4f).SetPosition(info.Position);
            }

            // Register player damage with the Karma system
            TTTReborn.Gamemode.Game.Instance?.Karma?.RegisterPlayerDamage(info.Attacker as TTTPlayer, this, info.Damage);

            _lastDamageInfo = info;

            base.TakeDamage(info);
        }
    }
}
