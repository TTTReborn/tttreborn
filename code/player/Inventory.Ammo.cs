// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;
using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Player
{
    public partial class AmmoInventory
    {
        private Dictionary<string, int> AmmoList { get; } = new();
        private readonly TTTPlayer _owner;

        public AmmoInventory(TTTPlayer owner)
        {
            _owner = owner;
        }

        public int Count(string ammoType)
        {
            string ammo = ammoType.ToLower();

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


            if (Host.IsServer)
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

            if (Host.IsServer)
            {
                _owner.ClientClearAmmo(To.Single(_owner));
            }
        }
    }
}
