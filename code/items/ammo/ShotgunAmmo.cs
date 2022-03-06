using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_ammo_shotgun")]
    [Spawnable]
    [Hammer.EditorModel("models/ammo/ammo_buckshot.vmdl")]
    public partial class ShotgunAmmo : Ammo
    {
        public override int Amount => 12;
        public override int Max => 36;
        public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";
    }
}
