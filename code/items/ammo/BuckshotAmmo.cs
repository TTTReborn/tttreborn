using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_buckshot")]
    partial class BuckshotAmmo : TTTAmmo
    {
        public override string AmmoType => "buckshot";
        public override int AmmoAmount => 12;
        public override int MaxAmmo => 36;
        public override string ModelPath => "models/ammo_buckshot.vmdl";
    }
}
