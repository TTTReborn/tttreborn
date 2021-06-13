using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Weapons;

// TODO Fix animation on dropping a higher slot weapon (instead of deleting and recreating, move and delete WeaponSlots in DOM)

namespace TTTReborn.UI
{
    public class WeaponSelection : Panel
    {
        private Dictionary<string, WeaponSlot> weaponSlots = new();

        private Weapon oldActiveWeapon;

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

            // update weapon slots, check for removed weapons etc.
            Inventory inventory = player.Inventory as Inventory;
            int inventoryCount = inventory.Count();
            List<Weapon> newWeapons = new();
            List<WeaponSlot> tmpSlots = new();

            for (int i = 0; i < inventoryCount; i++)
            {
                Weapon weapon = inventory.GetSlot(i) as Weapon;

                if (weaponSlots.TryGetValue(weapon.Name, out WeaponSlot weaponSlot))
                {
                    tmpSlots.Add(weaponSlot);
                }
                else
                {
                    newWeapons.Add(weapon);
                }

                // Do not update if we already rebuilding the WeaponSlots
                if (newWeapons.Count == 0)
                {
                    weaponSlot.UpdateAmmo($"{weapon.AmmoClip} + {player.AmmoCount(weapon.AmmoType)}");
                }
            }

            // remove WeaponSlots and rebuild (to keep the right order)
            if (newWeapons.Count != 0 || tmpSlots.Count != weaponSlots.Count)
            {
                foreach (WeaponSlot weaponSlot in weaponSlots.Values)
                {
                    weaponSlot.Delete();
                }

                weaponSlots.Clear();

                inventory.List.Sort((Entity wep1, Entity wep2) => (wep1 as Weapon).WeaponType.CompareTo((wep2 as Weapon).WeaponType));

                // rebuild weaponslots in the right order
                foreach (Weapon weapon in inventory.List)
                {
                    // add in order
                    weaponSlots.Add(weapon.Name, new WeaponSlot(this, weapon));
                }

                // update for dropped active weapon maybe
                oldActiveWeapon = null;
            }

            // update current selection
            Weapon activeWeapon = player.ActiveChild as Weapon;

            if (oldActiveWeapon != activeWeapon && activeWeapon != null)
            {
                foreach (WeaponSlot weaponSlot in weaponSlots.Values)
                {
                    weaponSlot.SetClass("active", weaponSlot.WeaponName == activeWeapon.Name);
                }

                oldActiveWeapon = activeWeapon;
            }

            SetClass("hide", weaponSlots.Count == 0);
        }

        /// <summary>
        /// IClientInput implementation, calls during the client input build.
        /// You can both read and write to input, to affect what happens down the line.
        /// </summary>
        [Event.BuildInput]
        private void ProcessClientWeaponSelectionInput(InputBuilder input)
        {
            Inventory inventory = Local.Pawn.Inventory as Inventory;

            if (inventory.Count() == 0)
            {
                return;
            }

            int selectedWeaponIndex = SlotPressInput(input);

            // no button pressed, but maybe scrolled
            if (selectedWeaponIndex == 0)
            {
                if (input.MouseWheel != 0)
                {
                    int nextSlot = inventory.List.IndexOf(Local.Pawn.ActiveChild) - input.MouseWheel;
                    nextSlot = inventory.NormalizeSlotIndex(nextSlot, inventory.Count() - 1);

                    input.ActiveChild = inventory.GetSlot(nextSlot);

                    input.MouseWheel = 0;
                }

                return;
            }

            // corresponding key pressed
            int nextWeaponSlot = inventory.GetNextWeaponSlot((WeaponType) selectedWeaponIndex);

            if (nextWeaponSlot != -1)
            {
                input.ActiveChild = inventory.GetSlot(nextWeaponSlot);
            }
        }

        // TODO: Handle mouse wheel, and additional number keys.
        private int SlotPressInput(InputBuilder input)
        {
            if (input.Pressed(InputButton.Slot1)) return 1;
            if (input.Pressed(InputButton.Slot2)) return 2;
            if (input.Pressed(InputButton.Slot3)) return 3;
            if (input.Pressed(InputButton.Slot4)) return 4;
            if (input.Pressed(InputButton.Slot5)) return 5;

            return 0;
        }

        public class WeaponSlot : Panel
        {
            public string WeaponName { get; private set; }
            public Label SlotLabel { set; get; }
            public Label WeaponLabel { set; get; }
            public Label AmmoLabel { set; get; }

            public WeaponSlot(Panel parent, Weapon weapon)
            {
                Parent = parent;
                WeaponName = weapon.Name;

                SlotLabel = Add.Label(((int) weapon.WeaponType).ToString(), "slotlabel");
                WeaponLabel = Add.Label(weapon.Name, "weaponlabel");
                AmmoLabel = Add.Label($"{weapon.AmmoClip}/{weapon.ClipSize}", "ammolabel");
            }

            public void UpdateAmmo(string ammoText)
            {
                AmmoLabel.Text = ammoText;
            }
        }
    }
}
