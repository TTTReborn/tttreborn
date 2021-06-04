using Sandbox;

namespace TTTReborn.Player
{
    public class Body : ModelEntity
    {
        public TTTPlayer Player { get; set; }
        public bool Identified { get; set; } = false;

        public Body()
        {
            MoveType = MoveType.Physics;
            UsePhysicsCollision = true;

            SetInteractsAs(CollisionLayer.Debris);
            SetInteractsWith(CollisionLayer.WORLD_GEOMETRY);
            SetInteractsExclude(CollisionLayer.Player | CollisionLayer.Debris);

            Identified = false;
        }

        public void CopyFrom(TTTPlayer player)
        {
            SetModel(player.GetModelName());
            TakeDecalsFrom(player);

            // We have to use `this` to refer to the extension methods.
            this.CopyBonesFrom(player);
            this.SetRagdollVelocityFrom(player);

            foreach (Entity child in player.Children)
            {
                if (child is ModelEntity e)
                {
                    string model = e.GetModelName();

                    if (model != null && !model.Contains("clothes"))
                    {
                        continue;
                    }

                    ModelEntity clothing = new ModelEntity();
                    clothing.SetModel(model);
                    clothing.SetParent(this, true);
                }
            }
        }

        public void ApplyForceToBone(Vector3 force, int forceBone)
        {
            PhysicsGroup.AddVelocity(force);

            if (forceBone >= 0)
            {
                var body = GetBonePhysicsBody(forceBone);

                if (body != null)
                {
                    body.ApplyForce(force * 1000);
                }
                else
                {
                    PhysicsGroup.AddVelocity(force);
                }
            }
        }
    }

}
