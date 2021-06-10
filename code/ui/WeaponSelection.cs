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
        List<Weapon> Weapons = new();
        Dictionary<string, WeaponSlot> WeaponSlots = new Dictionary<string, WeaponSlot>();

        Weapon SelectedWeapon;

        public WeaponSelection()
        {
            StyleSheet.Load("/ui/WeaponSelection.scss");
        }

        // TODO: This method is terrible performance, fix it.
        // Consider checking out DM98 or Hidden inventory HUD.
        public override void Tick()
        {
            base.Tick();

            var player = Local.Pawn as TTTPlayer;
            if (player == null) return;

            Weapons.Clear();
            Weapons.AddRange(player.Children.Select(x => x as Weapon).Where(x => x.IsValid()));

            SelectedWeapon = Local.Pawn.ActiveChild as Weapon;

            int index = 1;
            foreach (Weapon weapon in Weapons)
            {
                WeaponSlot weaponSlot = null;
                if (WeaponSlots.TryGetValue(weapon.Name, out weaponSlot))
                {
                    weaponSlot.UpdateWeaponSlot(index, weapon.Name, $"{weapon.AmmoClip}/{weapon.ClipSize}");
                    if (weapon == SelectedWeapon)
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
                    WeaponSlots.Add(weapon.Name, new WeaponSlot(this, index, weapon.Name, $"{weapon.AmmoClip}/{weapon.ClipSize}"));
                }
                index += 1;
            }
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        public void ProcessClientInput(InputBuilder input)
        {
	        if (Weapons.Count == 0)
        	{
	            return;
        	}

	        SelectedWeapon = Local.Pawn.ActiveChild as Weapon;

        	var oldSelected = SelectedWeapon;
        	int SelectedIndex = Weapons.IndexOf( SelectedWeapon );
        	SelectedIndex = SlotPressInput(input);

            SelectedWeapon = Weapons[SelectedIndex];

            foreach (Weapon weapon in Weapons)
            {
                WeaponSlot weaponSlot = null;
                if (WeaponSlots.TryGetValue(weapon.Name, out weaponSlot))
                {
                    if (weapon == SelectedWeapon)
                    {
                        weaponSlot.AddClass("active");
                    }
                    else
                    {
                        weaponSlot.RemoveClass("active");
                    }
                }
            }

            if ( oldSelected  != SelectedWeapon )
        	{
        		Sound.FromScreen( "dm.ui_tap" );
        	}
        }

        int SlotPressInput(InputBuilder input)
        {
            var columninput = 0;

            if ( input.Pressed( InputButton.Slot1 ) ) columninput = 0;
            if ( input.Pressed( InputButton.Slot2 ) ) columninput = 1;
            if ( input.Pressed( InputButton.Slot3 ) ) columninput = 2;
            if ( input.Pressed( InputButton.Slot4 ) ) columninput = 3;
            if ( input.Pressed( InputButton.Slot5 ) ) columninput = 4;

            return columninput;
        }

        public class WeaponSlot : Panel
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
