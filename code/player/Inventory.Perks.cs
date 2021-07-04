using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class PerksInventory
    {
        private List<TTTPerk> PerkList { get; set; } = new();
        private Inventory Inventory;

        public PerksInventory(Inventory inventory) : base()
        {
            Inventory = inventory;
        }

        public bool Give(TTTPerk perk)
        {
            if (Has(perk))
            {
                return false;
            }

            PerkList.Add(perk);

            TTTPlayer player = Inventory.Owner as TTTPlayer;

            perk.Equip(player);

            if (Host.IsServer)
            {
                player.ClientAddPerk(To.Single(player), perk.Name);
            }

            return true;
        }

        public bool Take(TTTPerk perk)
        {
            if (!Has(perk))
            {
                return false;
            }

            PerkList.Remove(perk);

            TTTPlayer player = Inventory.Owner as TTTPlayer;

            perk.Remove();
            perk.Delete();

            if (Host.IsServer)
            {
                player.ClientRemovePerk(To.Single(player), perk.Name);
            }

            return true;
        }

        public bool Has(TTTPerk perk)
        {
            return PerkList.Contains(perk);
        }

        public void Clear()
        {
            TTTPlayer player = Inventory.Owner as TTTPlayer;

            foreach (TTTPerk perk in PerkList)
            {
                perk.Remove();
                perk.Delete();
            }

            PerkList.Clear();

            if (Host.IsServer)
            {
                player.ClientClearPerks(To.Single(player));
            }
        }
    }
}
