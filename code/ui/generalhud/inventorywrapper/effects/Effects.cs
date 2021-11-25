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
using Sandbox.UI;

using TTTReborn.Items;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class Effects : Panel
    {
        private readonly List<Effect> _effectList = new();

        public Effects()
        {
            StyleSheet.Load("/ui/generalhud/inventorywrapper/effects/Effects.scss");

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            PerksInventory perks = player.Inventory.Perks;

            for (int i = 0; i < perks.Count(); i++)
            {
                AddEffect(perks.Get(i));
            }
        }

        public void AddEffect(TTTPerk perk)
        {
            _effectList.Add(new Effect(this, perk));
        }

        public void RemoveEffect(TTTPerk perk)
        {
            foreach (Effect effect in _effectList)
            {
                if (effect.Item.LibraryName == perk.LibraryName)
                {
                    _effectList.Remove(effect);
                    effect.Delete();

                    return;
                }
            }
        }
    }
}
