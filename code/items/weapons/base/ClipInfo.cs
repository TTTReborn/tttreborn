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
        public virtual int ClipSize { get; set; } = 10;

        public virtual int BulletsPerReload {
            get => _bulletsPerReload ?? ClipSize;
            set => _bulletsPerReload = value;
        }
        private int? _bulletsPerReload = null;

        public string AmmoName { get; set; } = null;

        public int StartAmmo { get; set; } = -1;

        public int Ammo { get; set; } = 0;

        public virtual float Damage { get; set; } = 5f;

        public virtual bool UnlimitedAmmo { get; set; } = false;

        public virtual bool CanDropAmmo { get; set; } = true;

        /// <summary>Rate Per Minute, firing speed (higher is faster)</summary>
        public virtual int RPM { get; set; } = 200;

        public virtual float ReloadTime { get; set; } = 3f;

        public virtual bool IsPartialReloading { get; set; } = false;

        public virtual string ReloadAnim { get; set; } = "b_reload";
        public virtual string ViewModelReloadAnim { get; set; } = "reload";
        public virtual string ViewModelReloadFinishAnim { get; set; }

        public virtual string ShootSound { get; set; } = null;

        public virtual string DryFireSound { get; set; } = null;

        public virtual int Bullets { get; set; } = 1;

        public virtual float Spread { get; set; } = 0.05f;

        public virtual float Force { get; set; } = 1.5f;

        public virtual float BulletSize { get; set; } = 0.1f;

        public virtual string ImpactEffect { get; set; } = null;

        public virtual DamageFlags DamageType { get; set; } = DamageFlags.Bullet;

        public virtual Dictionary<string, string> ShootEffectList { get; set; } = new()
        {
            { "particles/pistol_muzzleflash.vpcf", "muzzle" }
        };

        public virtual ShakeEffect ShakeEffect { get; set; } = null;

        public virtual FiringType FiringType { get; set; } = FiringType.SEMI;
    }
}
