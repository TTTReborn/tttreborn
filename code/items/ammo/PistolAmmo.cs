using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_pistol")]
    partial class PistolAmmo : TTTAmmo
    {
        public override string AmmoType => "pistol";
        public override int AmmoAmount => 12;
        public override int AmmoMax => 60;
        public override string ModelPath => "models/ammo_buckshot.vmdl";
    }
}
