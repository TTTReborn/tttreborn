using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_smg")]
    partial class SMGAmmo : TTTAmmo
    {
        public override string Type => "smg";
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo_buckshot.vmdl";
    }
}
