using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_pistol")]
    [Hammer.EditorModel("models/ammo/ammo_9mm.vmdl")]
    partial class PistolAmmo : TTTAmmo
    {
        public override string AmmoName => "pistol";
        public override int Amount => 12;
        public override int Max => 60;
        public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
    }
}
