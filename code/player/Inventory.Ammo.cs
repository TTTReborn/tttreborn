using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class AmmoInventory
    {
        private Dictionary<AmmoType, int> AmmoList { get; } = new();
        private readonly TTTPlayer _owner;

        public AmmoInventory(TTTPlayer owner)
        {
            _owner = owner;
        }

        public int Count(AmmoType ammoType)
        {
            if (AmmoList == null || !AmmoList.ContainsKey(ammoType))
            {
                return 0;
            }

            return AmmoList[ammoType];
        }

        public bool Set(AmmoType ammoType, int amount)
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

        public bool Give(AmmoType ammoType, int amount)
        {
            if (AmmoList == null)
            {
                return false;
            }

            Set(ammoType, Count(ammoType) + amount);

            return true;
        }

        public int Take(AmmoType ammoType, int amount)
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
