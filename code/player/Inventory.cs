using System;
using System.Linq;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class Inventory : BaseInventory
    {
        public readonly List<TTTPerk> Perks = new();

        public readonly Ammo Ammo;

        public Inventory(TTTPlayer player) : base(player)
        {
            Ammo = new Ammo(this);
        }

        public override void DeleteContents()
        {
            base.DeleteContents();

            TTTPlayer player = Owner as TTTPlayer;

            foreach (TTTPerk perk in Perks)
            {
                perk.Remove(player);
            }

            Perks.Clear();
            Ammo.Clear();
        }

        public override bool Add(Entity entity, bool makeActive = false)
        {
            TTTPlayer player = Owner as TTTPlayer;

            if (IsCarryingType(entity.GetType()))
            {
                return false;
            }

            if (entity is ICarriableItem)
            {
                Sound.FromWorld("dm.pickup_weapon", entity.Position);
            }

            bool added = base.Add(entity, makeActive);

            List.Sort((Entity carr1, Entity carr2) => (carr1 as ICarriableItem).HoldType.CompareTo((carr2 as ICarriableItem).HoldType));

            return added;
        }

        public bool Add(TTTPerk perk)
        {
            if (Perks.Contains(perk))
            {
                return false;
            }

            Perks.Add(perk);

            return true;
        }

        public bool Add(IItem item)
        {
            if (item is Entity ent)
            {
                return Add(ent);
            }
            else if (item is TTTPerk perk)
            {
                return Add(perk);
            }

            return false;
        }

        public bool IsCarryingType(Type t)
        {
            return List.Any(x => x.GetType() == t);
        }

        public int NormalizeSlotIndex(int index, int maxIndex)
        {
            return index > maxIndex ? 0 : (index < 0 ? maxIndex : index);
        }

        /// <summary>
        /// Returns the next inventory slot. If a `HoldType` is given and the corresponding key was pressed, the index of a different carriable entity with
        /// the same slot will be returned.
        /// </summary>
        /// <param name="holdType">Optional. If not set, the next carriable entity in the list will be returned (no matter the slot)</param>
        /// <returns>
        /// -1: No selection / carriable entity switch should proceed,
        /// >0: The index of the carriable entity in the inventory list that should get selected
        /// </returns>
        public int GetNextSlot(HoldType holdType = 0)
        {
            int inventorySize = Count();

            if (inventorySize < 1)
            {
                return -1;
            }

            int activeSlot = GetActiveSlot();

            if (activeSlot != -1)
            {
                int nextIndex = NormalizeSlotIndex(activeSlot + 1, inventorySize - 1); // Get next slot index
                ICarriableItem nextCarr = List[nextIndex] as ICarriableItem;

                if (holdType != 0)
                {
                    if (nextCarr.HoldType != holdType)
                    {
                        for (int carrIndex = 0; carrIndex < inventorySize; carrIndex++)
                        {
                            ICarriableItem indexCarr = List[carrIndex] as ICarriableItem;

                            if (indexCarr.HoldType == holdType)
                            {
                                return carrIndex;
                            }
                        }
                    }
                    else
                    {
                        // if there is no carriable entity with same slot or no slot defined, return the next available carriable entity
                        return nextIndex;
                    }

                    return -1;
                }
            }

            // edge case, if List does not contain the active carriable entity
            return 0;
        }

        public override bool Drop(Entity ent)
        {
            if (!Host.IsServer || !Contains(ent) || ent is ICarriableItem item && !item.CanDrop())
            {
                return false;
            }

            return base.Drop(ent);
        }
    }

    public partial class Ammo
    {
        private List<int> AmmoList { get; set; } = new();
        private Inventory Inventory;

        public Ammo(Inventory inventory) : base()
        {
            Inventory = inventory;
        }

        public int Count(AmmoType ammoType)
        {
            int iAmmoType = (int) ammoType;

            if (AmmoList == null || AmmoList.Count <= iAmmoType)
            {
                return 0;
            }

            return AmmoList[iAmmoType];
        }

        public bool Set(AmmoType ammoType, int amount)
        {
            int iAmmoType = (int) ammoType;

            if (AmmoList == null)
            {
                return false;
            }

            while (AmmoList.Count <= iAmmoType)
            {
                AmmoList.Add(0);
            }

            AmmoList[iAmmoType] = amount;

            if (Host.IsServer)
            {
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientSetAmmo(To.Single(player), ammoType, amount);
            }

            return true;
        }

        public bool Give(AmmoType ammoType, int amount)
        {
            if (AmmoList == null)
            {
                return false;
            }

            Set(ammoType, Count(ammoType) + amount);

            return true;
        }

        public int Take(AmmoType ammoType, int amount)
        {
            if (AmmoList == null)
            {
                return 0;
            }

            int available = Count(ammoType);
            amount = Math.Min(available, amount);

            Set(ammoType, available - amount);

            return amount;
        }

        public void Clear()
        {
            AmmoList.Clear();

            if (Host.IsServer)
            {
                TTTPlayer player = Inventory.Owner as TTTPlayer;

                player.ClientClearAmmo(To.Single(player));
            }
        }
    }
}
