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
                TTTReborn.Globals.RPCs.ClientOnPlayerCarriableItemPickup(To.Single(player), player, entity);
                Sound.FromWorld("dm.pickup_weapon", entity.Position);
            }

            bool added = base.Add(entity, makeActive);

            return added;
        }

        public bool Add(TTTPerk perk)
        {
            return Perks.Give(perk);
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

        public override bool Drop(Entity entity)
        {
            if (!Host.IsServer || !Contains(entity) || entity is ICarriableItem item && !item.CanDrop())
            {
                return false;
            }

            using (Prediction.Off())
            {
                TTTPlayer player = Owner as TTTPlayer;
                TTTReborn.Globals.RPCs.ClientOnPlayerCarriableItemDrop(To.Single(player), player, entity);
            }

            return base.Drop(entity);
        }
    }
}
