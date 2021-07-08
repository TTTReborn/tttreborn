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
            if (Has(perk.Name))
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
            if (!Has(perk.Name))
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

        public TTTPerk Find(string perkName)
        {
            foreach (TTTPerk loopPerk in PerkList)
            {
                if (perkName == loopPerk.Name)
                {
                    return loopPerk;
                }
            }

            return null;
        }

        public bool Has(string perkName)
        {
            return Find(perkName) != null;
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
