using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_offensive")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_9mm.vmdl")]
    public partial class OffensiveEquipmentAmmo : Ammo
    {
        public override int Amount => 2;
        public override int Max => 4;
        public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
    }
}
