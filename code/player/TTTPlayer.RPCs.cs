using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;

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
    }
}
