using System.ComponentModel.DataAnnotations;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_offensiveequipment")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_9mm.vmdl")]
    [Display(Name = "Offensive Equipment Ammo", GroupName = "Ammunition")]
    public partial class OffensiveEquipmentAmmo : Ammo
    {
        public override int Amount => 2;
        public override int Max => 4;
        public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
    }
}
