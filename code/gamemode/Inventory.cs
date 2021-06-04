using Sandbox;
using System;
using System.Linq;

using TTTReborn.Player;
using TTTReborn.Weapons;

namespace TTTReborn.Gamemode
{
    partial class Inventory : BaseInventory
    {
        public Inventory(TTTPlayer player) : base(player)
        {

        }

        public override bool Add(Entity entity, bool makeActive = false)
        {
            TTTPlayer player = Owner as TTTPlayer;

            if (entity is Weapon weapon && IsCarryingType(entity.GetType()))
            {
                int ammo = weapon.AmmoClip;
                AmmoType ammoType = weapon.AmmoType;

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
