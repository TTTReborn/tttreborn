using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("equipment_hands")]
    [Equipment(SlotType = SlotType.UtilityEquipment)]
    [Hammer.Skip]
    partial class Hands : TTTEquipment
    {
        public override string ViewModelPath => "";
        public override bool CanDrop() => false;

        private const float MAX_PICKUP_MASS = 205;
        private const float MAX_PICKUP_DISTANCE = 75;
        private Vector3 MAX_PICKUP_SIZE = new(50, 50, 50);

        public Entity GrabbedEntity { get; set; }

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;
        }

        public override void Simulate(Client client)
        {
            if (!IsServer)
            {
                return;
            }

            if (Owner is not TTTPlayer player)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    GrabEntity(player);
                }
                else if (Input.Released(InputButton.Attack2))
                {
                    DropEntity();
                }
            }
        }

        private void GrabEntity(TTTPlayer player)
        {
            if (GrabbedEntity.IsValid())
            {
                return;
            }

            Vector3 eyePos = player.EyePos;
            Vector3 eyeDir = player.EyeRot.Forward;

            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * MAX_PICKUP_DISTANCE)
                .UseHitboxes()
                .Ignore(player)
                .HitLayer(CollisionLayer.Debris)
                .EntitiesOnly()
                .Run();

            // Make sure trace is hit and not null.
            if (!tr.Hit || !tr.Entity.IsValid())
            {
                return;
            }

            // Cannot pickup players or objects currently being held.
            if (tr.Entity is TTTPlayer || tr.Entity.Parent != null)
            {
                return;
            }

            // Has to be a model, smaller collision box than MAX_PICKUP_SIZE, mass less than MAX_PICKUP_MASS
            if (tr.Entity is not ModelEntity model || model.CollisionBounds.Size.HasGreatorOrEqualAxis(MAX_PICKUP_SIZE) || tr.Entity.PhysicsGroup.Mass > MAX_PICKUP_MASS)
            {
                return;
            }

            tr.Entity.SetParent(player, "middle_of_both_hands", new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
            GrabbedEntity = tr.Entity;
            GrabbedEntity.EnableHideInFirstPerson = false;
        }

        private void DropEntity()
        {
            if (!GrabbedEntity.IsValid())
            {
                return;
            }

            GrabbedEntity.SetParent(null);
            GrabbedEntity = null;
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            DropEntity();

            base.ActiveEnd(ent, dropped);
        }

        protected override void OnDestroy()
        {
            DropEntity();

            base.OnDestroy();
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            if (!IsServer)
            {
                return;
            }

            if (GrabbedEntity.IsValid())
            {
                anim.SetParam("holdtype", 4);
                anim.SetParam("holdtype_handedness", 0);
            }
            else
            {
                anim.SetParam("holdtype", 0);
            }
        }
    }
}


