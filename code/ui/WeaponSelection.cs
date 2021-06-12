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
        private Dictionary<string, WeaponSlot> _weaponSlots = new();

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

            Weapon selectedWeapon = Local.Pawn.ActiveChild as Weapon;

            foreach(KeyValuePair<int, List<Weapon>> keyValuePair in player.WeaponSelection)
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
