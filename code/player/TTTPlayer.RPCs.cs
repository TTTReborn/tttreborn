using Sandbox;

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
        public void ClientOnPlayerCarriableItemPickup(Entity carriable)
        {
            Event.Run("tttreborn.player.carriableitem.pickup", carriable as ICarriableItem);
        }

        [ClientRpc]
        public void ClientOnPlayerCarriableItemDrop(Entity carriable)
        {
            Event.Run("tttreborn.player.carriableitem.drop", carriable as ICarriableItem);
        }

        [ClientRpc]
        public void ClientClearInventory()
        {
            Event.Run("tttreborn.player.inventory.clear");
        }

        [ClientRpc]
        public void ClientSetAmmo(AmmoType ammoType, int amount)
        {
            (Inventory as Inventory).Ammo.Set(ammoType, amount);
        }

        [ClientRpc]
        public void ClientClearAmmo()
        {
            (Inventory as Inventory).Ammo.Clear();
        }

        [ClientRpc]
        public void ClientAddPerk(string perkName)
        {
            TTTPerk perk = Utils.GetObjectByType<TTTPerk>(Utils.GetTypeByName<TTTPerk>(perkName));

            if (perk == null)
            {
                return;
            }

            (Inventory as Inventory).TryAdd(perk);
        }

        [ClientRpc]
        public void ClientRemovePerk(string perkName)
        {
            TTTPerk perk = Utils.GetObjectByType<TTTPerk>(Utils.GetTypeByName<TTTPerk>(perkName));

            if (perk == null)
            {
                return;
            }

            (Inventory as Inventory).Perks.Take(perk);
        }

        [ClientRpc]
        public void ClientClearPerks()
        {
            (Inventory as Inventory).Perks.Clear();
        }

        [ClientRpc]
        public void ClientOpenInspectMenu(TTTPlayer deadPlayer, bool isIdentified, ConfirmationData confirmationData, string killerWeapon = null)
        {
            if (!deadPlayer.IsValid())
            {
                return;
            }

            InspectMenu.Instance.InspectCorpse(deadPlayer, isIdentified, confirmationData, killerWeapon);
        }

        [ClientRpc]
        public void ClientCloseInspectMenu()
        {
            if (InspectMenu.Instance?.IsShowing ?? false)
            {
                InspectMenu.Instance.IsShowing = false;
            }
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
            Event.Run("tttreborn.player.takedamage", this, damage);
        }
    }
}
