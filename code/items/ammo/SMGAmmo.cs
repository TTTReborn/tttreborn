using Hammer;

using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_smg")]
    [EditorModel("models/ammo/ammo_smg.vmdl")]
    partial class SMGAmmo : TTTAmmo
    {
        public override string ClassName => "smg";
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
