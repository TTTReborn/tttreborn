using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class AmmoInventory
    {
        private Dictionary<string, int> AmmoList { get; set; } = new();
        private Inventory Inventory;

        public AmmoInventory(Inventory inventory) : base()
        {
            Inventory = inventory;
        }

        public bool RegisterType(string ammoType, int amount = 0)
        {
            var ammo = ammoType.ToLower();
            if (string.IsNullOrEmpty(ammo))
            {
                return false;
            }
            if (!AmmoList.ContainsKey(ammo))
            {
                AmmoList.Add(ammo, amount);
            }

            if (Host.IsServer)
            {
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientRegisterAmmo(To.Single(player), ammo, amount);
            }
            return true;
        }

        public int Count(string ammoType)
        {
            var ammo = ammoType.ToLower();

            if (AmmoList == null || !AmmoList.ContainsKey(ammo))
            {
                return 0;
            }

            return AmmoList[ammo];
        }

        public bool Set(string ammoType, int amount)
        {
            var ammo = ammoType.ToLower();

            if (AmmoList == null)
            {
                return false;
            }

            while (!AmmoList.ContainsKey(ammo))
            {
                AmmoList.Add(ammo, 0);
            }

            AmmoList[ammo] = amount;


            if (Host.IsServer)
            {
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientSetAmmo(To.Single(player), ammo, amount);
            }

            return true;
        }

        public bool Give(string ammoType, int amount)
        {
            var ammo = ammoType.ToLower();

            if (AmmoList == null)
            {
                return false;
            }

            Set(ammo, Count(ammo) + amount);

            return true;
        }

        public int Take(string ammoType, int amount)
        {
            var ammo = ammoType.ToLower();

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
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientClearAmmo(To.Single(player));
            }
        }
    }
}
