using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public class GrabbableEntities
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
                if (!GrabbedEntity.IsValid())
                {
                    Drop();
                    return;
                }

                // If the entity gets destroyed.
                if (GrabbedEntity.Health == 0)
                {
                    Drop();
                    return;
                }

                // If the entity is a weapon and gets picked up by another player.
                if (GrabbedEntity is TTTWeapon weapon && weapon.Owner != null)
                {
                    Drop();
                    return;
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
}
