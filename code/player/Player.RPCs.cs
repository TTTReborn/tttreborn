using Sandbox;

using TTTReborn.Events;
using TTTReborn.Items;

namespace TTTReborn
{
    public partial class Player
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
            Perk perk = Utils.GetObjectByType<Perk>(Utils.GetTypeByLibraryName<Perk>(perkName));

            if (perk == null)
            {
                return;
            }

            Inventory.TryAdd(perk, deleteIfFails: true, makeActive: false);
        }

        [ClientRpc]
        public void ClientRemovePerk(string perkName)
        {
            Perk perk = Utils.GetObjectByType<Perk>(Utils.GetTypeByLibraryName<Perk>(perkName));

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
            Event.Run(TTTEvent.Player.TAKE_DAMAGE, this, damage);
        }


        [ClientRpc]
        public void ClientInitialSpawn()
        {
            Event.Run(TTTEvent.Player.INITIAL_SPAWN, Client);
        }
    }
}
