using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTTGamemode
{
    public class Body : ModelEntity
    {
        public TTTPlayer TttPlayer { get; set; }
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

        public void CopyFrom(TTTPlayer tttPlayer)
        {
            SetModel(tttPlayer.GetModelName());
            TakeDecalsFrom(tttPlayer);

            // We have to use `this` to refer to the extension methods.
            this.CopyBonesFrom(tttPlayer);
            this.SetRagdollVelocityFrom(tttPlayer);

            foreach (var child in tttPlayer.Children)
            {
                if (child is ModelEntity e)
                {
                    var model = e.GetModelName();

                    if (model != null && !model.Contains("clothes"))
                        continue;

                    var clothing = new ModelEntity();
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
