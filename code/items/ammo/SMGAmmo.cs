using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_smg")]
    partial class SMGAmmo : TTTAmmo
    {
        public override string AmmoType => "smg";
        public override int AmmoAmount => 30;
        public override int MaxAmmo => 90;
        public override string ModelPath => "models/ammo_buckshot.vmdl";
    }
}
