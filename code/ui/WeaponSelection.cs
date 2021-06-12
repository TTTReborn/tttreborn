using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Weapons;

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
        // TODO: Update slot position when a new weapon is picked up
        public override void Tick()
        {
            base.Tick();

            var player = Local.Pawn as TTTPlayer;

            if (player == null)
            {
                return;
            }

            // update weapon slots, check for removed weapons etc.
            List<WeaponSlot> tmpSlots = new();

            Inventory inventory = player.Inventory as Inventory;
            int inventoryCount = inventory.Count();
            int added = 0;

            for (int i = 0; i < inventoryCount; i++)
            {
                Weapon weapon = inventory.GetSlot(i) as Weapon;

                weaponSlots.TryGetValue(weapon.Name, out WeaponSlot weaponSlot);

                if (weaponSlot == null)
                {
                    weaponSlot = new WeaponSlot(this, weapon);
                    weapon.WeaponSlotPanel = weaponSlot;

                    added++;
                }

                tmpSlots.Add(weaponSlot);
            }

            // remove invalid WeaponSlots
            if (tmpSlots.Count != weaponSlots.Count || added != 0)
            {
                foreach (WeaponSlot weaponSlot in weaponSlots.Values)
                {
                    if (!tmpSlots.Contains(weaponSlot))
                    {
                        weaponSlot.Delete();
                    }
                }

                weaponSlots.Clear();

                // setup
                foreach (WeaponSlot weaponSlot in tmpSlots)
                {
                    weaponSlots.Add(weaponSlot.WeaponName, weaponSlot);
                }
            }

            // update current selection
            Weapon activeWeapon = player.ActiveChild as Weapon;

            if (oldActiveWeapon != activeWeapon && activeWeapon != null)
            {
                foreach (WeaponSlot weaponSlot in weaponSlots.Values)
                {
                    if (weaponSlot.WeaponName == activeWeapon.Name)
                    {
                        weaponSlot.AddClass("active");
                    }
                    else
                    {
                        weaponSlot.RemoveClass("active");
                    }
                }

                oldActiveWeapon = activeWeapon;
            }
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
