using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_buckshot")]
    partial class BuckshotAmmo : TTTAmmo
    {
        public override string Type => "buckshot";
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo_buckshot.vmdl";
    }
}
