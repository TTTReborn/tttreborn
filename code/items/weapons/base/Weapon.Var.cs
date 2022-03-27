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

        public ClipInfo CurrentClip
        {
            get => _currentClip;
            set
            {
                _currentClip = value;

                if (_currentClip != null)
                {
                    ClipAmmo = _currentClip.Ammo;
                }
            }
        }
        private ClipInfo _currentClip = null;

        [Net, Predicted]
        public bool IsReloading { get; set; }

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

        [Net, Predicted]
        public int ClipAmmo { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceAttack { get; set; }

        [Net, Predicted]
        public TimeSince TimeSinceReload { get; set; }

        [Net, Predicted]
        public float CurrentReloadTime { get; set; }

        public bool CanZoom { get; set; }
    }
}
