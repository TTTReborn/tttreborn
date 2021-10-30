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

        private GrabbableEntities.IGrabbable GrabbedEntity;
        private bool IsHoldingEntity => GrabbedEntity != null && (GrabbedEntity?.IsHolding ?? false);

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;
        }

        public override void Simulate(Client client)
        {
            base.Simulate(client);

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
                    TryGrabEntity(player);
                }
                else if (Input.Pressed(InputButton.Attack2))
                {
                    GrabbedEntity?.Drop();
                }
                else if (Input.Pressed(InputButton.Use))
                {
                    GrabbedEntity?.SecondaryAction();
                }

                GrabbedEntity?.Update(player);
            }
        }

        private void TryGrabEntity(TTTPlayer player)
        {
            if (IsHoldingEntity)
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

            // Cannot pickup items held by other players.
            if (tr.Entity.Parent != null)
            {
                return;
            }

            // Ignore any size/mass restrictions when picking up a corpse.
            if (tr.Entity is PlayerCorpse)
            {
                GrabbedEntity = new GrabbableEntities.Ragdoll();
                GrabbedEntity.Grab(player, tr);

                return;
            }

            // Has to be a model, smaller collision box than MAX_PICKUP_SIZE, mass less than MAX_PICKUP_MASS
            if (tr.Entity is not ModelEntity model || model.CollisionBounds.Size.HasGreatorOrEqualAxis(MAX_PICKUP_SIZE) || tr.Entity.PhysicsGroup.Mass > MAX_PICKUP_MASS)
            {
                return;
            }
        }

        public override void ActiveEnd(Entity ent, bool dropped)
        {
            GrabbedEntity?.Drop();

            base.ActiveEnd(ent, dropped);
        }

        protected override void OnDestroy()
        {
            GrabbedEntity?.Drop();

            base.OnDestroy();
        }

        public override void SimulateAnimator(PawnAnimator anim)
        {
            if (!IsServer)
            {
                return;
            }

            if (IsHoldingEntity)
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
