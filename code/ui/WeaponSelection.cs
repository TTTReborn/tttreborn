//using System.Collections.Generic;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class WeaponSelection : Panel
    {
        //List<WeaponSlot> weaponSlots = new();

        public WeaponSelection()
        {
            StyleSheet.Load("/ui/WeaponSelection.scss");

            new WeaponSlot(this, 1, "Weapon 1", "0/0").AddClass("active");
            new WeaponSlot(this, 2, "Weapon 2", "0/0").AddClass("traitor");
            new WeaponSlot(this, 3, "Weapon 3", "0/0");
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
        }
    }

}
