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

            if (Host.IsServer)
            {
                player.ClientAddPerk(To.Single(player), perk.Name);
            }

            perk.Equip(player);

            return true;
        }

        public bool Take(TTTPerk perk)
        {
            if (!Has(perk.Name))
            {
                return false;
            }

            PerkList.Remove(perk);

            perk.Remove();
            perk.Delete();

            if (Host.IsServer)
            {
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientRemovePerk(To.Single(player), perk.Name);
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

                if (perkName == loopPerk.Name)
                {
                    return (T) loopPerk;
                }
                else if (perkName == null)
                {
                    return t;
                }
            }

            return default(T);
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
