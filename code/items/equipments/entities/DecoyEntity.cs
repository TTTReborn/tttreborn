using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_decoy_ent")]
    [Hammer.Skip]
    public partial class DecoyEntity : Prop
    {
        private string ModelPath => "models/entities/decoy.vmdl";

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        }
    }
}
