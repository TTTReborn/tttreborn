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

            bool added = base.Add(entity, makeActive);

            List.Sort((Entity wep1, Entity wep2) => {
                return (wep1 as Weapon).WeaponType.CompareTo((wep2 as Weapon).WeaponType);
            });

            return added;
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
        /// Returns the next weapon slot. If a `WeaponType` is given and the corresponding key was pressed, the index of a different weapon with
        /// the same slot will be returned.
        /// </summary>
        /// <param name="weaponType">Optional. If not set, the next weapon in the list will be returned (no matter the slot)</param>
        /// <returns>
        /// -1: No selection / weapon switch should proceed,
        /// >0: The index of the weapon in the inventory list that should get selected
        /// </returns>
        public int GetNextWeaponSlot(WeaponType weaponType = 0)
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
                Weapon nextWeapon = List[nextIndex] as Weapon;

                if (weaponType != 0)
                {
                    if (nextWeapon.WeaponType != weaponType)
                    {
                        for (int weaponIndex = 0; weaponIndex < inventorySize; weaponIndex++)
                        {
                            Weapon indexWeapon = List[weaponIndex] as Weapon;

                            if (indexWeapon.WeaponType == weaponType)
                            {
                                return weaponIndex;
                            }
                        }
                    }
                    else
                    {
                        // if there is no weapon with same slot or no slot defined, return the next available weapon
                        return nextIndex;
                    }

                    return -1;
                }
            }

            // edge case, if List does not contain the active weapon
            return 0;
        }
    }
}
