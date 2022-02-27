using Sandbox;

namespace TTTReborn.Items
{
    [Library("ammo_shotgun")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_buckshot.vmdl")]
    public partial class ShotgunAmmo : TTTAmmo
    {
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";
    }
}
