using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TTTReborn.Player;
using TTTReborn.Weapons;

namespace TTTReborn.UI
{
    public class WeaponSelection : Panel
    {
        private readonly SortedDictionary<int, List<Weapon>> _weaponsDict = new();
        private readonly Dictionary<string, WeaponSlot> _weaponSlots = new();

        public WeaponSelection()
        {
            StyleSheet.Load("/ui/WeaponSelection.scss");
        }

        // TODO: This method could be made to be event based. Whenever a user pickups a weapon, and whenever a user fires.
        // Consider checking out DM98 or Hidden inventory HUD.
        // TODO: Update slot position when a new weapon is picked up
        public override void Tick()
        {
            base.Tick();

            var player = Local.Pawn as TTTPlayer;

            if (player == null)
            {
                return;
            }

            _weaponsDict.Clear();

            foreach (Weapon weapon in player.Children)
            {
                if (!weapon.IsValid())
                {
                    continue;
                }

                int weaponType = (int) weapon.WeaponType;

                _weaponsDict.TryGetValue(weaponType, out List<Weapon> weaponList);

                if (weaponList == null)
                {
                    weaponList = new();

                    _weaponsDict.Add(weaponType, weaponList);
                }

                weaponList.Add(weapon);
            }

            Weapon selectedWeapon = Local.Pawn.ActiveChild as Weapon;

            foreach(KeyValuePair<int, List<Weapon>> keyValuePair in _weaponsDict)
            {
                foreach (Weapon weapon in keyValuePair.Value)
                {
                    if (_weaponSlots.TryGetValue(weapon.Name, out WeaponSlot weaponSlot))
                    {
                        weaponSlot.UpdateWeaponSlot(keyValuePair.Key, weapon.Name, $"{weapon.AmmoClip}/{weapon.ClipSize}");

                        if (weapon == selectedWeapon)
                        {
                            weaponSlot.AddClass("active");
                        }
                        else
                        {
                            weaponSlot.RemoveClass("active");
                        }
                    }
                    else
                    {
                        _weaponSlots.Add(weapon.Name, new WeaponSlot(this, keyValuePair.Key, weapon.Name, $"{weapon.AmmoClip}/{weapon.ClipSize}"));
                    }
                }
            }
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        public void ProcessClientInput(InputBuilder input)
        {
	        if (_weaponsDict.Count == 0)
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
            _weaponsDict.TryGetValue(weaponSlot, out List<Weapon> weaponList);

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

        private class WeaponSlot : Panel
        {
            public Label SlotLabel { set; get; }
            public Label WeaponLabel { set; get; }
            public Label AmmoLabel { set; get; }

            public WeaponSlot(Panel parent, int slotId, string weaponName, string ammo)
            {
                Parent = parent;

                SlotLabel = Add.Label(slotId.ToString(), "slotlabel");
                WeaponLabel = Add.Label(weaponName, "weaponlabel");
                AmmoLabel = Add.Label(ammo, "ammolabel");
            }

            public void UpdateWeaponSlot(int slotId, string weaponName, string ammo)
            {
                SlotLabel.Text = slotId.ToString();
                WeaponLabel.Text = weaponName;
                AmmoLabel.Text = ammo;
            }
        }
    }

}
