using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_rifle")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_smg.vmdl")]
    partial class RifleAmmo : TTTAmmo
    {
        public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Rifle;
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
