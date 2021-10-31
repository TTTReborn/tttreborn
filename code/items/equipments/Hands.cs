using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IGrabbable
    {
        const string MIDDLE_HANDS_ATTACHMENT = "middle_of_both_hands";

        bool IsHolding { get; }
        void Drop();
        void Update(TTTPlayer player);
        void SecondaryAction();
    }

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

        private IGrabbable GrabbedEntity;
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

            switch (tr.Entity)
            {
                case PlayerCorpse:
                    GrabbedEntity = new GrabbableCorpse(player, tr.Body);
                    break;
                case ModelEntity model:
                    if (!model.CollisionBounds.Size.HasGreatorOrEqualAxis(MAX_PICKUP_SIZE) && model.PhysicsGroup.Mass < MAX_PICKUP_MASS)
                    {
                        GrabbedEntity = new GrabbableProp(player, tr.Entity);
                    }
                    break;
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
