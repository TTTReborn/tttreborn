using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Player
{
    public partial class PlayerCorpse : ModelEntity
    {
        public TTTPlayer Player { get; set; }

        public List<Particles> Ropes = new();

        public List<PhysicsJoint> RopeSprings = new();

        [Net]
        public bool IsIdentified { get; set; } = false;

        public PlayerCorpse()
        {
            MoveType = MoveType.Physics;
            UsePhysicsCollision = true;

            SetInteractsAs(CollisionLayer.Debris);
            SetInteractsWith(CollisionLayer.WORLD_GEOMETRY);
            SetInteractsExclude(CollisionLayer.Player | CollisionLayer.Debris);

            IsIdentified = false;
        }

        public void CopyFrom(TTTPlayer player)
        {
            SetModel(player.GetModelName());
            TakeDecalsFrom(player);

            this.CopyBonesFrom(player);
            this.SetRagdollVelocityFrom(player);

            foreach (Entity child in player.Children)
            {
                if (child is ModelEntity e)
                {
                    string model = e.GetModelName();

                    if (model == null || !model.Contains("clothes"))
                    {
                        continue;
                    }

                    ModelEntity clothing = new();
                    clothing.SetModel(model);
                    clothing.SetParent(this, true);
                }
            }
        }

        public void ApplyForceToBone(Vector3 force, int forceBone)
        {
            PhysicsGroup.AddVelocity(force);

            if (forceBone < 0)
            {
                return;
            }

            PhysicsBody corpse = GetBonePhysicsBody(forceBone);

            if (corpse != null)
            {
                corpse.ApplyForce(force * 1000);
            }
            else
            {
                PhysicsGroup.AddVelocity(force);
            }
        }

        public void ClearAttachments()
        {
            foreach (Particles rope in Ropes)
            {
                rope.Destroy(true);
            }

            foreach (PhysicsJoint spring in RopeSprings)
            {
                spring.Remove();
            }

            Ropes.Clear();
            RopeSprings.Clear();
        }

        protected override void OnDestroy()
        {
            ClearAttachments();
        }
    }
}
