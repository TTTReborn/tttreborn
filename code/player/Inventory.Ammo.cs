using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class AmmoInventory
    {
        private List<int> AmmoList { get; set; } = new();
        private Inventory Inventory;

        public AmmoInventory(Inventory inventory) : base()
        {
            Inventory = inventory;
        }

        public int Count(AmmoType ammoType)
        {
            int iAmmoType = (int) ammoType;

            if (AmmoList == null || AmmoList.Count <= iAmmoType)
            {
                return 0;
            }

            return AmmoList[iAmmoType];
        }

        public bool Set(AmmoType ammoType, int amount)
        {
            int iAmmoType = (int) ammoType;

            if (AmmoList == null)
            {
                return false;
            }

            while (AmmoList.Count <= iAmmoType)
            {
                AmmoList.Add(0);
            }

            AmmoList[iAmmoType] = amount;

            if (Host.IsServer)
            {
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientSetAmmo(To.Single(player), ammoType, amount);
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
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientClearAmmo(To.Single(player));
            }
        }
    }
}
