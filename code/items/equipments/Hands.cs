using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IGrabbable
    {
        bool IsHolding { get; }
        void Drop();
        void Update(TTTPlayer player);
        void SecondaryAction();
    }

    [Library("equipment_hands")]
    [Equipment(SlotType = SlotType.UtilityEquipment)]
    [Precached("particles/rope.vpcf")]
    [Hammer.Skip]
    partial class Hands : TTTEquipment
    {
        public override string ViewModelPath => "";
        public override bool CanDrop() => false;
        public static readonly float MAX_INTERACT_DISTANCE = 75;
        public static readonly string MIDDLE_HANDS_ATTACHMENT = "middle_of_both_hands";

        private const float MAX_PICKUP_MASS = 205;
        private Vector3 MAX_PICKUP_SIZE = new(50, 50, 50);
        private const float PUSHING_FORCE = 600f;

        private IGrabbable GrabbedEntity;
        private bool IsHoldingEntity => GrabbedEntity != null && (GrabbedEntity?.IsHolding ?? false);
        private bool IsPushingPlayer = false;

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
                    if (IsHoldingEntity)
                    {
                        GrabbedEntity?.Drop();
                    }
                    else
                    {
                        PushPlayer(player);
                    }
                }
                else if (Input.Pressed(InputButton.Reload) && IsHoldingEntity)
                {
                    GrabbedEntity?.SecondaryAction();
                }

                GrabbedEntity?.Update(player);
            }
        }

        private void PushPlayer(TTTPlayer player)
        {
            TraceResult tr = Trace.Ray(player.EyePos, player.EyePos + player.EyeRot.Forward * MAX_INTERACT_DISTANCE)
                    .Ignore(player)
                    .Run();

            if (!tr.Hit || tr.Entity is not TTTPlayer || !tr.Entity.IsValid())
            {
                return;
            }

            IsPushingPlayer = true;

            player.SetAnimBool("b_attack", true);
            player.SetAnimInt("holdtype", 4);
            player.SetAnimInt("holdtype_handedness", 0);

            tr.Entity.Velocity += player.EyeRot.Forward * PUSHING_FORCE;

            _ = WaitForAnimationFinish();
        }

        private async Task WaitForAnimationFinish()
        {
            await GameTask.DelaySeconds(0.6f);
            IsPushingPlayer = false;
        }

        private void TryGrabEntity(TTTPlayer player)
        {
            if (IsHoldingEntity)
            {
                return;
            }

            Vector3 eyePos = player.EyePos;
            Vector3 eyeDir = player.EyeRot.Forward;

            TraceResult tr = Trace.Ray(eyePos, eyePos + eyeDir * MAX_INTERACT_DISTANCE)
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
                    GrabbedEntity = new GrabbableCorpse(player, tr.Entity as PlayerCorpse, tr.Body, tr.Bone);
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

            if (IsPushingPlayer)
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
