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
        private readonly List<Weapon> _weapons = new();
        private readonly Dictionary<string, WeaponSlot> _weaponSlots = new();

        private Weapon _selectedWeapon;

        public WeaponSelection()
        {
            StyleSheet.Load("/ui/WeaponSelection.scss");
        }

        // TODO: This method could be made to be event based. Whenever a user pickups a weapon, and whenever a user fires.
        // Consider checking out DM98 or Hidden inventory HUD.
        public override void Tick()
        {
            base.Tick();

            var player = Local.Pawn as TTTPlayer;

            if (player == null)
            {
                return;
            }

            _weapons.Clear();
            _weapons.AddRange(player.Children.Select(x => x as Weapon).Where(x => x.IsValid()));

            _selectedWeapon = Local.Pawn.ActiveChild as Weapon;

            int weaponIndex = 1;

            foreach (Weapon weapon in _weapons)
            {
                WeaponSlot weaponSlot = null;

                if (_weaponSlots.TryGetValue(weapon.Name, out weaponSlot))
                {
                    weaponSlot.UpdateWeaponSlot(weaponIndex, weapon.Name, $"{weapon.AmmoClip}/{weapon.ClipSize}");

                    if (weapon == _selectedWeapon)
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
                    _weaponSlots.Add(weapon.Name, new WeaponSlot(this, weaponIndex, weapon.Name, $"{weapon.AmmoClip}/{weapon.ClipSize}"));
                }

                weaponIndex += 1;
            }
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        public void ProcessClientInput(InputBuilder input)
        {
	        if (_weapons.Count == 0)
        	{
	            return;
        	}

            _selectedWeapon = Local.Pawn.ActiveChild as Weapon;

            int selectedWeaponIndex = SlotPressInput(input);

            if (selectedWeaponIndex == -1)
            {
                return;
            }

            input.ActiveChild = _weapons[selectedWeaponIndex];
        }

        // TODO: Handle mouse wheel, and additional number keys.
        int SlotPressInput(InputBuilder input)
        {
            var weaponIndex = -1;

            if (input.Pressed(InputButton.Slot1)) weaponIndex = 0;
            if (input.Pressed(InputButton.Slot2)) weaponIndex = 1;
            if (input.Pressed(InputButton.Slot3)) weaponIndex = 2;
            if (input.Pressed(InputButton.Slot4)) weaponIndex = 3;
            if (input.Pressed(InputButton.Slot5)) weaponIndex = 4;

            return weaponIndex;
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
