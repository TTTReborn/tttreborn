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

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        [ClientRpc]
        private void ClientShowFlashlightLocal(bool shouldShow)
        {
            ShowFlashlight(shouldShow);
        }

        [ClientRpc]
        public void ClientSetAmmo(string ammoType, int amount)
        {
            Inventory.Ammo.Set(ammoType, amount);
        }

        [ClientRpc]
        public void ClientClearAmmo()
        {
            Inventory.Ammo.Clear();
        }

        [ClientRpc]
        public void ClientAddPerk(string perkName)
        {
            TTTPerk perk = Utils.GetObjectByType<TTTPerk>(Utils.GetTypeByLibraryName<TTTPerk>(perkName));

            if (perk == null)
            {
                return;
            }

            Inventory.TryAdd(perk, deleteIfFails: true, makeActive: false);
        }

        [ClientRpc]
        public void ClientRemovePerk(string perkName)
        {
            TTTPerk perk = Utils.GetObjectByType<TTTPerk>(Utils.GetTypeByLibraryName<TTTPerk>(perkName));

            if (perk == null)
            {
                return;
            }

            Inventory.Perks.Take(perk);
        }

        [ClientRpc]
        public void ClientClearPerks()
        {
            Inventory.Perks.Clear();
        }

        [ClientRpc]
        public void ClientAnotherPlayerDidDamage(Vector3 position, float inverseHealth)
        {
            Sound.FromScreen("dm.ui_attacker")
                .SetPitch(1 + inverseHealth * 1)
                .SetPosition(position);
        }

        [ClientRpc]
        public void ClientTookDamage(Vector3 position, float damage)
        {
            Event.Run(TTTEvent.Player.TakeDamage, this, damage);
        }


        [ClientRpc]
        public void ClientInitialSpawn()
        {
            Event.Run(TTTEvent.Player.InitialSpawn, Client);
        }
    }
}
