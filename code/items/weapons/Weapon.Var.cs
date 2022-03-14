using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        [Net]
        public ItemInfo Info { get; set; } = new WeaponInfo();

        public WeaponInfo WeaponInfo
        {
            get => Info as WeaponInfo;
        }

        [Net]
        public ClipInfo Primary { get; set; } = new();

        [Net]
        public ClipInfo Secondary { get; set; } = null;

        [Net]
        public TimeSince TimeSinceReload { get; set; }

        public bool IsReloading
        {
            get => Primary.IsReloading || (Secondary?.IsReloading ?? false);
        }

        [Net]
        public TimeSince TimeSinceDeployed { get; set; }

        public PickupTrigger PickupTrigger { get; set; }

        private const int AMMO_DROP_POSITION_OFFSET = 50;
        private const int AMMO_DROP_VELOCITY = 500;

        [Net]
        public Entity LastDropOwner { get; set; }

        [Net]
        public TimeSince SinceLastDrop { get; set; } = 0f;

        public abstract string ModelPath { get; }
    }
}
