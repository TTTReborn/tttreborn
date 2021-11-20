using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class AmmoInventory
    {
        private Dictionary<AmmoTypes, int> AmmoList { get; } = new();
        private readonly TTTPlayer _owner;

        public AmmoInventory(TTTPlayer owner)
        {
            _owner = owner;
        }

        public int Count(AmmoTypes ammoType)
        {
            if (AmmoList == null || !AmmoList.ContainsKey(ammoType))
            {
                return 0;
            }

            return AmmoList[ammoType];
        }

        public bool Set(AmmoTypes ammoType, int amount)
        {
            if (AmmoList == null)
            {
                return false;
            }

            while (!AmmoList.ContainsKey(ammoType))
            {
                AmmoList.Add(ammoType, 0);
            }

            AmmoList[ammoType] = amount;


            if (Host.IsServer)
            {
                _owner.ClientSetAmmo(To.Single(_owner), ammoType, amount);
            }

            return true;
        }

        public bool Give(AmmoTypes ammoType, int amount)
        {
            if (AmmoList == null)
            {
                return false;
            }

            Set(ammoType, Count(ammoType) + amount);

            return true;
        }

        public int Take(AmmoTypes ammoType, int amount)
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

            if (Host.IsServer)
            {
                _owner.ClientClearAmmo(To.Single(_owner));
            }
        }
    }
}
