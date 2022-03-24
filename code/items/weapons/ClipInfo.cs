using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Items
{
    public enum FiringType
    {
        /// <summary>Single fire</summary>
        SEMI,
        /// <summary>Automatic fire</summary>
        AUTO,
        /// <summary>3-Burst fire</summary>
        BURST
    }

    public struct ShootEffect
    {
        public readonly string Name;
        public readonly string Attachment;

        public ShootEffect(string name, string attachment)
        {
            Name = name;
            Attachment = attachment;
        }
    }

    public partial class ShakeEffect
    {
        public float Length { get; set; } = 0f;
        public float Speed { get; set; } = 0f;
        public float Size { get; set; } = 0f;
        public float Rotation { get; set; } = 0f;
    }

    public partial class ClipInfo
    {
        public int ClipSize { get; set; } = 10;

        public string AmmoName { get; set; } = "";

        public int ClipAmmo { get; set; } = 10;

        public float Damage { get; set; } = 5f;

        public bool UnlimitedAmmo { get; set; } = false;

        public bool CanDropAmmo { get; set; } = true;

        /// <summary>Rate Per Minute, firing speed (higher is faster)</summary>
        public int RPM { get; set; } = 200;

        public bool IsReloading { get; set; } = false;

        public TimeSince TimeSinceAttack { get; set; } = 0f;

        public string ShootSound { get; set; } = null;

        public string DryFireSound { get; set; } = null;

        public int Bullets { get; set; } = 1;

        public float Spread { get; set; } = 0.05f;

        public float Force { get; set; } = 1.5f;

        public float BulletSize { get; set; } = 0.1f;

        public string ImpactEffect { get; set; } = null;

        public DamageFlags DamageType = DamageFlags.Bullet;

        public Dictionary<string, string> ShootEffectList { get; set; } = new()
        {
            { "particles/pistol_muzzleflash.vpcf", "muzzle" }
        };

        public ShakeEffect ShakeEffect { get; set; } = null;

        public FiringType FiringType { get; set; } = FiringType.SEMI;
    }
}
