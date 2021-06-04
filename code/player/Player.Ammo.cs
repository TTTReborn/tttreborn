using Sandbox;
using System;
using System.Collections.Generic;

using TTTReborn.Weapons;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [Net]
        public List<int> Ammo { get; set; } = new();

        public void ClearAmmo()
        {
            Ammo.Clear();
        }

        public int AmmoCount(AmmoType type)
        {
            int iType = (int)type;

            if (Ammo == null || Ammo.Count <= iType)
            {
                return 0;
            }

            return Ammo[iType];
        }

        public bool SetAmmo(AmmoType type, int amount)
        {
            int iType = (int)type;

            if (!Host.IsServer || Ammo == null)
            {
                return false;
            }

            while (Ammo.Count <= iType)
            {
                Ammo.Add(0);
            }

            Ammo[iType] = amount;

            return true;
        }

        public bool GiveAmmo(AmmoType type, int amount)
        {
            if (!Host.IsServer || Ammo == null)
            {
                return false;
            }

            SetAmmo(type, AmmoCount(type) + amount);

            return true;
        }

        public int TakeAmmo(AmmoType type, int amount)
        {
            if (Ammo == null)
            {
                return 0;
            }

            int available = AmmoCount(type);
            amount = Math.Min(available, amount);

            SetAmmo(type, available - amount);

            return amount;
        }
    }

}
