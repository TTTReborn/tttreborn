using System;
using System.Linq;
using Sandbox;

using TTTReborn.Weapons;

namespace TTTReborn.Player
{
    partial class Inventory : BaseInventory
    {
        public Inventory(TTTPlayer player) : base(player)
        {

        }

        public override bool Add(Entity entity, bool makeActive = false)
        {
            TTTPlayer player = Owner as TTTPlayer;

            if (IsCarryingType(entity.GetType()))
            {
                return false;
            }

            if (entity is Weapon weapon)
            {
                Sound.FromWorld("dm.pickup_weapon", entity.Position);
            }

            return base.Add(entity, makeActive);
        }

        public bool IsCarryingType(Type t)
        {
            return List.Any(x => x.GetType() == t);
        }

        int GetNextIndex(int index, int maxIndex)
        {
            return NormalizeSlotIndex(index + 1, maxIndex);
        }

        public int NormalizeSlotIndex(int index, int maxIndex)
        {
            return index > maxIndex ? 0 : (index < 0 ? maxIndex : index);
        }

        public int GetNextWeaponSlot(WeaponType weaponType = 0)
        {
            int listSize = Count();

            if (listSize < 1)
            {
                return -1;
            }

            int activeSlot = GetActiveSlot();

            if (activeSlot != -1)
            {
                int nextIndex = GetNextIndex(activeSlot, listSize - 1);
                Weapon nextWeapon = List[nextIndex] as Weapon;

                if (weaponType != 0 && nextWeapon.WeaponType != weaponType)
                {
                    for (int weaponIndex = 0; weaponIndex < listSize; weaponIndex++)
                    {
                        Weapon indexWeapon = List[weaponIndex] as Weapon;

                        if (indexWeapon.WeaponType == weaponType)
                        {
                            return weaponIndex;
                        }
                    }
                }

                // if there is no weapon with same slot or no slot defined, return the next available weapon
                return nextIndex;
            }

            // edge case, if List does not contain the active weapon
            return 0;
        }
    }
}
