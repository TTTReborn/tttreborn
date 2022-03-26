using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public virtual ItemInfo Info { get; private set; } = new WeaponInfo();

        public virtual WeaponInfo WeaponInfo
        {
            get => Info as WeaponInfo;
            set => Info = value;
        }

        public virtual ClipInfo Primary { get; set; } = new();

        public virtual ClipInfo Secondary { get; set; } = null;

        public bool IsReloading
        {
            get => IsPrimaryReloading || Secondary != null && IsSecondaryReloading;
        }

        public virtual float BulletRange { get; set; } = 20000f;

        [Net, Predicted]
        public TimeSince TimeSinceDeployed { get; set; }

        public PickupTrigger PickupTrigger { get; set; }

        private const int AMMO_DROP_POSITION_OFFSET = 50;
        private const int AMMO_DROP_VELOCITY = 500;

        [Net]
        public Entity LastDropOwner { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceLastDrop { get; set; }

        public abstract string ModelPath { get; }

        // Bullshit design, but s&box class networking is unreliable
        [Net, Predicted]
        public int PrimaryClipAmmo { get; set; }

        [Net, Predicted]
        public int SecondaryClipAmmo { get; set; }

        [Net, Predicted]
        public TimeSince TimeSincePrimaryAttack { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceSecondaryAttack { get; set; }

        [Net, Predicted]
        public TimeSince TimeSincePrimaryReload { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceSecondaryReload { get; set; }

        [Net, Predicted]
        public bool IsPrimaryReloading { get; set; }

        [Net, Predicted]
        public bool IsSecondaryReloading { get; set; }
    }
}
