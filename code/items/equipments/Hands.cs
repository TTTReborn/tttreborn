using Sandbox;
using Sandbox.Joints;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    class GrabbableEntities
    {
        private const string MIDDLE_HANDS_ATTACHMENT = "middle_of_both_hands";

        public interface IGrabbable
        {
            bool IsHolding { get; }
            void Grab(TTTPlayer player, TraceResult tr);
            void Drop();
            void Update(TTTPlayer player);
            void SecondaryAction();
        }

        public class Prop : IGrabbable
        {
            public Entity GrabbedEntity { get; set; }

            public bool IsHolding
            {
                get => GrabbedEntity != null;
            }

            public void Grab(TTTPlayer player, TraceResult tr)
            {
                GrabbedEntity = tr.Entity;
                GrabbedEntity.SetParent(player, MIDDLE_HANDS_ATTACHMENT, new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
                GrabbedEntity.EnableHideInFirstPerson = false;
            }

            public void Drop()
            {
                if (!GrabbedEntity.IsValid())
                {
                    return;
                }

                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity.SetParent(null);
                GrabbedEntity = null;
            }

            public void Update(TTTPlayer player)
            {
                // If the weapon is picked up, we should stop holding it.
                if (GrabbedEntity is TTTWeapon weapon && weapon.Owner != null)
                {
                    Drop();
                }

                // If the entity gets destroyed, we should stop holding it.
                if (GrabbedEntity?.Health == 0)
                {
                    Drop();
                }
            }

            public void SecondaryAction() { }
        }

        public class Ragdoll : IGrabbable
        {
            private PhysicsBody _handPhysicsBody;
            private WeldJoint _holdJoint;

            public bool IsHolding
            {
                get => _holdJoint.IsValid;
            }

            public Ragdoll()
            {
                _handPhysicsBody = PhysicsWorld.AddBody();
                _handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;
            }

            public void Grab(TTTPlayer player, TraceResult tr)
            {
                Transform attachment = player.GetAttachment(MIDDLE_HANDS_ATTACHMENT)!.Value;
                _handPhysicsBody.Position = attachment.Position;
                _handPhysicsBody.Rotation = attachment.Rotation;

                _holdJoint = PhysicsJoint.Weld
                    .From(_handPhysicsBody)
                    .To(tr.Body)
                    .Create();
            }

            public void Drop()
            {
                if (_holdJoint.IsValid)
                {
                    _holdJoint.Remove();
                }

                _handPhysicsBody = null;
            }

            public void Update(TTTPlayer player)
            {
                if (_handPhysicsBody == null)
                {
                    return;
                }

                Transform attachment = player.GetAttachment("middle_of_both_hands")!.Value;
                _handPhysicsBody.Position = attachment.Position;
                _handPhysicsBody.Rotation = attachment.Rotation;
            }

            public void SecondaryAction() { }
        }
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

            GrabbedEntity = new GrabbableEntities.Prop();
            GrabbedEntity.Grab(player, tr);
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
