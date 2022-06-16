using System.ComponentModel.DataAnnotations;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_pistol")]
    [Spawnable]
    [EditorModel("models/ammo/ammo_9mm.vmdl")]
    [Display(Name = "Pistol Ammo", GroupName = "Ammunition")]
    [Title("Pistol Ammo")]
    public partial class PistolAmmo : Ammo
    {
        public override int Amount => 12;
        public override int Max => 60;
        public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
    }
}
