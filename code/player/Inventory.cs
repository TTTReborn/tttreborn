using Sandbox;
using System;
using System.Linq;

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

            if (entity is Weapon weapon)
            {
                if (IsCarryingType(entity.GetType()))
                {
                    int ammo = weapon.AmmoClip;
                    AmmoType ammoType = weapon.AmmoType;

                    if (ammo > 0)
                    {
                        player.GiveAmmo(ammoType, ammo);

                        Sound.FromWorld("dm.pickup_ammo", entity.Position);
                    }

                    entity.Delete();

                    return false;
                }

                Sound.FromWorld("dm.pickup_weapon", entity.Position);
            }

            return base.Add(entity, makeActive);
        }

        public bool IsCarryingType(Type t)
        {
            return List.Any(x => x.GetType() == t);
        }

        public void ProcessClientWeaponSelectionInput(InputBuilder input)
        {
            if (List.Count == 0)
        	{
	            return;
        	}

            int selectedWeaponIndex = SlotPressInput(input);

            if (selectedWeaponIndex == 0)
            {
                return;
            }

            int nextWeaponSlot = GetNextWeaponSlot((WeaponType) selectedWeaponIndex);

            if (nextWeaponSlot != -1)
            {
                SetActiveSlot(nextWeaponSlot);
            }
        }

        // TODO: Handle mouse wheel, and additional number keys.
        int SlotPressInput(InputBuilder input)
        {
            if (input.Pressed(InputButton.Slot1)) return 1;
            if (input.Pressed(InputButton.Slot2)) return 2;
            if (input.Pressed(InputButton.Slot3)) return 3;
            if (input.Pressed(InputButton.Slot4)) return 4;
            if (input.Pressed(InputButton.Slot5)) return 5;

            return 0;
        }

        int GetNextIndex(int index, int maxIndex)
        {
            return index + 1 >= maxIndex ? 0 : index + 1;
        }

        private int GetNextWeaponSlot(WeaponType weaponType = 0)
        {
            int listSize = Count();

            if (listSize < 1)
            {
                return -1;
            }

            int activeSlot = GetActiveSlot();

            if (activeSlot != -1)
            {
                int nextIndex = GetNextIndex(activeSlot, listSize);
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

		/// <summary>
		/// Drop this entity. Will return true if successfully dropped.
		/// </summary>
		public override bool Drop(Entity ent)
		{
			if (!Host.IsServer || !Contains(ent))
            {
                return false;
            }

            Weapon activeWeapon = (Owner.ActiveChild as Weapon);

            int nextWeaponSlot = GetNextWeaponSlot(activeWeapon.WeaponType);

            if (nextWeaponSlot != -1)
            {
                SetActiveSlot(nextWeaponSlot);
            }

			ent.Parent = null;
			ent.OnCarryDrop(Owner);

			return true;
		}

		/// <summary>
		/// Delete every entity we're carrying. Useful to call on death.
		/// </summary>
		public override void DeleteContents()
		{
			Host.AssertServer();

			foreach (Weapon weapon in List.ToArray())
			{
				weapon.Delete();
			}

			List.Clear();
		}
    }
}
