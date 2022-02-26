using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_pistol")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_9mm.vmdl")]
    public partial class PistolAmmo : TTTAmmo
    {
        public override int Amount => 12;
        public override int Max => 60;
        public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
    }
}
