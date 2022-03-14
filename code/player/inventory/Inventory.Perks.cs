using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn
{
    public partial class PerksInventory
    {
        private List<Perk> PerkList { get; } = new();
        private readonly Player _owner;

        internal PerksInventory(Player owner)
        {
            _owner = owner;
        }

        public bool Give(Perk perk)
        {
            if (Has(perk.Info.LibraryName))
            {
                return false;
            }

            PerkList.Add(perk);

            if (Host.IsServer)
            {
                _owner.ClientAddPerk(To.Single(_owner), perk.Info.LibraryName);
            }

            perk.Equip(_owner);

            return true;
        }

        public bool Take(Perk perk)
        {
            if (!Has(perk.Info.LibraryName))
            {
                return false;
            }

            PerkList.Remove(perk);

            perk.Remove();
            perk.Delete();

            if (Host.IsServer)
            {
                _owner.ClientRemovePerk(To.Single(_owner), perk.Info.LibraryName);
            }

            return true;
        }

        public T Find<T>(string perkName = null) where T : Perk
        {
            foreach (Perk loopPerk in PerkList)
            {
                if (loopPerk is not T t || t.Equals(default(T)))
                {
                    continue;
                }

                if (perkName == t.Info.LibraryName)
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

        public Perk Find(string perkName)
        {
            return Find<Perk>(perkName);
        }

        public bool Has(string perkName = null)
        {
            return Find(perkName) != null;
        }

        public bool Has<T>(string perkName = null) where T : Perk
        {
            return Find<T>(perkName) != null;
        }

        public void Clear()
        {
            foreach (Perk perk in PerkList)
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

        public Perk Get(int index)
        {
            return PerkList[index];
        }
    }
}
