using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_sniper")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_9mm.vmdl")]
    public partial class SniperAmmo : Ammo
    {
        public override int Amount => 12;
        public override int Max => 24;
        public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
    }
}
