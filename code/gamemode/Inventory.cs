using Sandbox;
using System;
using System.Linq;
using System.Collections.Generic;

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

                int weaponType = (int) weapon.WeaponType;

                player.WeaponSelection.TryGetValue(weaponType, out List<Weapon> weaponList);

                if (weaponList == null)
                {
                    weaponList = new();

                    player.WeaponSelection.Add(weaponType, weaponList);
                }

                weaponList.Add(weapon);
            }

            return base.Add(entity, makeActive);
        }

        public bool Remove(Entity entity)
        {
            TTTPlayer player = Owner as TTTPlayer;

            if (entity is Weapon weapon)
            {
                int weaponType = (int) weapon.WeaponType;

                player.WeaponSelection.TryGetValue(weaponType, out List<Weapon> weaponList);

                if (weaponList == null)
                {
                    return false;
                }

                weaponList.Remove(weapon);

                if (weaponList.Count() == 0)
                {
                    player.WeaponSelection.Remove(weaponType);
                }

                return true;
            }

            return false;
        }

        public override void DeleteContents()
        {
            (Owner as TTTPlayer).WeaponSelection.Clear();

            base.DeleteContents();
        }

        public bool IsCarryingType(Type t)
        {
            return List.Any(x => x.GetType() == t);
        }

        public void ProcessClientWeaponSelectionInput(InputBuilder input)
        {
            TTTPlayer player = Owner as TTTPlayer;

	        if (player.WeaponSelection.Count == 0)
        	{
	            return;
        	}

            int selectedWeaponIndex = SlotPressInput(input);

            if (selectedWeaponIndex == 0)
            {
                return;
            }

            Weapon nextWeapon = GetNextWeapon(selectedWeaponIndex, Local.Pawn.ActiveChild as Weapon);

            if (nextWeapon != null)
            {
                input.ActiveChild = nextWeapon;
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

        private Weapon GetNextWeapon(int weaponSlot, Weapon currentWeapon)
        {
            TTTPlayer player = Owner as TTTPlayer;

            player.WeaponSelection.TryGetValue(weaponSlot, out List<Weapon> weaponList);

            if (weaponList == null)
            {
                return null;
            }

            int listSize = weaponList.Count();

            if (listSize < 1)
            {
                return null;
            }

            for (int i = 0; i < listSize; i++)
            {
                Weapon weapon = weaponList[i];

                if (weapon == currentWeapon)
                {
                    return weaponList[i + 1 >= listSize ? 0 : i + 1];
                }
            }

            return weaponList[0];
        }

        [Event("tttreborn.player.droppedweapon")]
        public void OnDropWeapon(Weapon weapon)
        {
            TTTPlayer player = Owner as TTTPlayer;

            Log.Warning("Dropped Weapon");

            int weaponSlot = (int) weapon.WeaponType;

            Weapon nextWeapon = GetNextWeapon(weaponSlot, weapon);

            if (nextWeapon != null)
            {
                player.ActiveChild = nextWeapon;
            }
            else
            {
                // no weapon of same type in slot
                foreach(KeyValuePair<int, List<Weapon>> keyValuePair in player.WeaponSelection)
                {
                    foreach (Weapon wep in keyValuePair.Value)
                    {
                        player.ActiveChild = wep;

                        break;
                    }

                    if (player.ActiveChild != null)
                    {
                        break;
                    }
                }
            }
        }
    }
}
