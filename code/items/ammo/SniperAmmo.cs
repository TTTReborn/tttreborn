using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_sniper")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_smg.vmdl")]
    partial class SniperAmmo : TTTAmmo
    {
        public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Sniper;
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
