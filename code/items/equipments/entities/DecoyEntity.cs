using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_entity_decoy")]
    [Precached("models/entities/decoy.vmdl")]
    [HideInEditor]
    public partial class DecoyEntity : Prop
    {
        public override string ModelPath => "models/entities/decoy.vmdl";

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        }
    }
}
