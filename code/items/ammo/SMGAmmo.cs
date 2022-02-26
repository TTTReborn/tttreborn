using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_smg")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_smg.vmdl")]
    public partial class SMGAmmo : TTTAmmo
    {
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
