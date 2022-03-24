using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public ItemInfo Info { get; set; } = new WeaponInfo();

        public WeaponInfo WeaponInfo
        {
            get => Info as WeaponInfo;
        }

        public virtual ClipInfo Primary { get; set; } = new();

        public virtual ClipInfo Secondary { get; set; } = null;

        public TimeSince TimeSinceReload { get; set; }

        public bool IsReloading
        {
            get => Primary.IsReloading || (Secondary?.IsReloading ?? false);
        }

        public virtual float BulletRange { get; set; } = 20000f;

        public virtual bool IsPartialReloading { get; set; }

        public TimeSince TimeSinceDeployed { get; set; }

        public PickupTrigger PickupTrigger { get; set; }

        private const int AMMO_DROP_POSITION_OFFSET = 50;
        private const int AMMO_DROP_VELOCITY = 500;

        public Entity LastDropOwner { get; set; }

        public TimeSince TimeSinceLastDrop { get; set; }

        public abstract string ModelPath { get; }
    }
}
