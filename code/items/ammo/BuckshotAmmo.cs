using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_shotgun")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_buckshot.vmdl")]
    partial class ShotgunAmmo : TTTAmmo
    {
        public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Shotgun;
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";
    }
}
