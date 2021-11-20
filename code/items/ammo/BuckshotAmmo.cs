using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_buckshot")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_buckshot.vmdl")]
    partial class BuckshotAmmo : TTTAmmo
    {
        public override AmmoType AmmoType => AmmoType.Buckshot;
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";
    }
}
