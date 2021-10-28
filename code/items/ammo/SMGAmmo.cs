using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_smg")]
    [Hammer.EditorModel("models/ammo/ammo_smg.vmdl")]
    partial class SMGAmmo : TTTAmmo
    {
        public override string AmmoName => "smg";
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
