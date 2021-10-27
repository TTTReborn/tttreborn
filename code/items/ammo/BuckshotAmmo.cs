using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_buckshot")]
    [Hammer.EditorModel("models/ammo/ammo_buckshot.vmdl")]
    partial class BuckshotAmmo : TTTAmmo
    {
        public override string AmmoName => "buckshot";
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";
    }
}
