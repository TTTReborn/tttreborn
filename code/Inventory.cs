using Sandbox;
using System;
using System.Linq;

namespace TTTGamemode
{
    partial class Inventory : BaseInventory
    {
        public Inventory(Player player) : base(player)
        {

        }

        public override bool Add(Entity entity, bool makeActive = false)
        {
            var player = Owner as Player;
            var weapon = entity as Weapon;

            if (weapon != null && IsCarryingType(entity.GetType()))
            {
                var ammo = weapon.AmmoClip;
                var ammoType = weapon.AmmoType;

                if (ammo > 0)
                {
                    player.GiveAmmo(ammoType, ammo);
                }

                entity.Delete();

                return false;
            }

            return base.Add(entity, makeActive);
        }

        public bool IsCarryingType(Type t)
        {
            return List.Any(x => x.GetType() == t);
        }
    }
}
