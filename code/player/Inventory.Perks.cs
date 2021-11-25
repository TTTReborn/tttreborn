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

using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class PerksInventory
    {
        private List<TTTPerk> PerkList { get; } = new();
        private readonly TTTPlayer _owner;

        public PerksInventory(TTTPlayer owner)
        {
            _owner = owner;
        }

        public bool Give(TTTPerk perk)
        {
            if (Has(perk.LibraryName))
            {
                return false;
            }

            PerkList.Add(perk);

            if (Host.IsServer)
            {
                _owner.ClientAddPerk(To.Single(_owner), perk.LibraryName);
            }

            perk.Equip(_owner);

            return true;
        }

        public bool Take(TTTPerk perk)
        {
            if (!Has(perk.LibraryName))
            {
                return false;
            }

            PerkList.Remove(perk);

            perk.Remove();
            perk.Delete();

            if (Host.IsServer)
            {
                _owner.ClientRemovePerk(To.Single(_owner), perk.LibraryName);
            }

            return true;
        }

        public T Find<T>(string perkName = null) where T : TTTPerk
        {
            foreach (TTTPerk loopPerk in PerkList)
            {
                if (loopPerk is not T t || t.Equals(default(T)))
                {
                    continue;
                }

                if (perkName == t.LibraryName)
                {
                    return t;
                }

                if (perkName == null)
                {
                    return t;
                }
            }

            return default;
        }

        public TTTPerk Find(string perkName)
        {
            return Find<TTTPerk>(perkName);
        }

        public bool Has(string perkName = null)
        {
            return Find(perkName) != null;
        }

        public bool Has<T>(string perkName = null) where T : TTTPerk
        {
            return Find<T>(perkName) != null;
        }

        public void Clear()
        {
            foreach (TTTPerk perk in PerkList)
            {
                perk.Remove();
                perk.Delete();
            }

            PerkList.Clear();

            if (Host.IsServer)
            {
                _owner.ClientClearPerks(To.Single(_owner));
            }
        }

        public int Count()
        {
            return PerkList.Count;
        }

        public TTTPerk Get(int index)
        {
            return PerkList[index];
        }
    }
}
