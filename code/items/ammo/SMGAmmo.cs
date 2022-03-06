using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_smg")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_smg.vmdl")]
    public partial class SMGAmmo : Ammo
    {
        public override int Amount => 30;
        public override int Max => 90;
        public override string ModelPath => "models/ammo/ammo_smg.vmdl";
    }
}
