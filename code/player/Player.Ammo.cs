using Sandbox;
using System;

namespace TTTGamemode
{
    partial class Player
    {
        [Net] public NetList<int> Ammo { get; set; } = new();

        public void ClearAmmo()
        {
            Ammo.Clear();
        }

        public int AmmoCount(AmmoType type)
        {
            if (Ammo == null)
                return 0;

            return Ammo.Get(type);
        }

        public bool GiveAmmo(AmmoType type, int amount)
        {
            if (!Host.IsServer || Ammo == null)
                return false;

            return Ammo.Set(type, AmmoCount(type) + amount);
        }

        public int TakeAmmo(AmmoType type, int amount)
        {
            var available = Ammo.Get(type);
            amount = Math.Min(Ammo.Get(type), amount);

            Ammo.Set(type, available - amount);
            NetworkDirty("Ammo", NetVarGroup.Net);

            return amount;
        }
    }
}
