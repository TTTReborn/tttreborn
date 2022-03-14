using Sandbox;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
        public ItemInfo Info { get; set; }

        public WeaponInfo WeaponInfo
        {
            get => Info as WeaponInfo;
        }

        public ClipInfo Primary { get; set; }

        public ClipInfo Secondary { get; set; }

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
        public TimeSince SinceLastDrop { get; set; }

        public abstract string ModelPath { get; }
    }
}
