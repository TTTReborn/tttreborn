using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_pistol")]
    partial class PistolAmmo : TTTAmmo
    {
        public override string Type => "pistol";
        public override int Amount => 12;
        public override int Max => 60;
        public override string ModelPath => "models/ammo_buckshot.vmdl";
    }
}
