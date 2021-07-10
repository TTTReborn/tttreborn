using System;
using System.Linq;

using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public partial class Inventory : BaseInventory
    {
        public readonly PerksInventory Perks;

        public readonly AmmoInventory Ammo;

        public Inventory(TTTPlayer player) : base(player)
        {
            Ammo = new AmmoInventory(this);
            Perks = new PerksInventory(this);
        }

        public override void DeleteContents()
        {
            foreach (Entity entity in List)
            {
                if (entity is IItem item)
                {
                    item.Remove();
                }
            }

            base.DeleteContents();

            TTTPlayer player = Owner as TTTPlayer;

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

            if (entity is ICarriableItem carriable)
            {
                player.ClientOnPlayerCarriableItemPickup(To.Single(player), entity);
                Sound.FromWorld("dm.pickup_weapon", entity.Position);
            }

            bool added = base.Add(entity, makeActive);

            return added;
        }

        public bool Add(TTTPerk perk)
        {
            return Perks.Give(perk);
        }

        public bool Add(IItem item, bool makeActive = false)
        {
            if (item is Entity ent)
            {
                return Add(ent, makeActive);
            }
            else if (item is TTTPerk perk)
            {
                return Add(perk);
            }

            return false;
        }

        /// <summary>
        /// Tries to add an `TTTReborn.Items.IItem` to the inventory. If it fails, the given item is deleted
        /// </summary>
        /// <param name="item">`TTTReborn.Items.IItem` that will be added to the inventory or get removed on fail</param>
        /// <param name="makeActive"></param>
        /// <returns></returns>
        public bool TryAdd(IItem item, bool makeActive = false)
        {
            if (!Add(item, makeActive))
            {
                item.Delete();

                return false;
            }

            return true;
        }

        public bool IsCarryingType(Type t)
        {
            return List.Any(x => x.GetType() == t);
        }

        public override bool Drop(Entity entity)
        {
            if (!Host.IsServer || !Contains(entity) || entity is ICarriableItem item && !item.CanDrop())
            {
                return false;
            }

            using (Prediction.Off())
            {
                TTTPlayer player = Owner as TTTPlayer;
                player.ClientOnPlayerCarriableItemDrop(To.Single(player), entity);
            }

            return base.Drop(entity);
        }
    }
}
