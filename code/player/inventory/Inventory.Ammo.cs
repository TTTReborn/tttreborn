using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn
{
    public partial class AmmoInventory
    {
        private Dictionary<string, int> AmmoList { get; } = new();
        private readonly Player _owner;

        internal AmmoInventory(Player owner)
        {
            _owner = owner;
        }

        public int Count(string ammoName)
        {
            if (ammoName == null)
            {
                return 0;
            }

            string ammo = ammoName.ToLower();

            if (AmmoList == null || !AmmoList.ContainsKey(ammo))
            {
                return 0;
            }

            return AmmoList[ammo];
        }

        public bool Set(string ammoType, int amount)
        {
            string ammo = ammoType.ToLower();

            if (AmmoList == null || string.IsNullOrEmpty(ammo))
            {
                return false;
            }

            while (!AmmoList.ContainsKey(ammo))
            {
                AmmoList.Add(ammo, 0);
            }

            AmmoList[ammo] = amount;


            if (Game.IsServer)
            {
                _owner.ClientSetAmmo(To.Single(_owner), ammo, amount);
            }

            return true;
        }

        public bool Give(string ammoType, int amount)
        {
            string ammo = ammoType.ToLower();

            if (AmmoList == null)
            {
                return false;
            }

            Set(ammo, Count(ammo) + amount);

            return true;
        }

        public int Take(string ammoType, int amount)
        {
            if (AmmoList == null)
            {
                return 0;
            }

            int available = Count(ammoType);
            amount = Math.Min(available, amount);

            Set(ammoType, available - amount);

            return amount;
        }

        public void Clear()
        {
            AmmoList.Clear();

            if (Game.IsServer)
            {
                _owner.ClientClearAmmo(To.Single(_owner));
            }
        }
    }
}
