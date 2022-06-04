using System.ComponentModel.DataAnnotations;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_smg")]
    [Spawnable]
    [EditorModel("models/ammo/ammo_smg.vmdl")]
    [Display(Name = "SMG Ammo", GroupName = "Ammunition")]
    [Title("SMG Ammo")]
    public partial class SMGAmmo : Ammo
    {
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
