using Sandbox;

namespace TTTReborn.Items
{
    [Hammer.Skip]
    [Library("ttt_decoy_ent")]
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
