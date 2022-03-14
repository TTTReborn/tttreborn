using System.Collections.Generic;
using System;

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

    public struct ShakeEffect
    {
        public readonly float Length;
        public readonly float Speed;
        public readonly float Size;
        public readonly float Rotation;

        public ShakeEffect(float length = 1f, float speed = 1f, float size = 1f, float rotation = 0.6f)
        {
            Length = length;
            Speed = speed;
            Size = size;
            Rotation = rotation;
        }
    }

    public partial class ClipInfo : BaseNetworkable
    {
        [Net]
        public int ClipSize { get; set; } = 10;

        [Net]
        public string AmmoName { get; set; } = "";

        [Net]
        public int ClipAmmo { get; set; } = 10;

        [Net]
        public float Damage { get; set; } = 5f;

        public Type AmmoType { get; set; }

        [Net]
        public bool UnlimitedAmmo { get; set; } = false;

        [Net]
        public bool CanDropAmmo { get; set; } = true;

        /// <summary>Rate Per Minute, firing speed (higher is faster)</summary>
        [Net]
        public int RPM { get; set; } = 200;

        [Net]
        public bool IsReloading { get; set; } = false;

		[Net, Predicted]
		public TimeSince TimeSinceAttack { get; set; } = 0f;

        [Net]
        public string ShootSound { get; set; }

        [Net]
        public string DryFireSound { get; set; }

        [Net]
        public int Bullets { get; set; } = 1;

        [Net]
        public float Spread { get; set; } = 0.05f;

        [Net]
        public float Force { get; set; } = 1.5f;

        [Net]
        public float BulletSize { get; set; } = 0.1f;

        public List<ShootEffect> ShootEffectList { get; set; } = new()
        {
            new("particles/pistol_muzzleflash.vpcf", "muzzle")
        };

        public ShakeEffect ShakeEffect { get; set; } = new();

        [Net]
        public FiringType FiringType { get; set; } = FiringType.SEMI;
    }
}
