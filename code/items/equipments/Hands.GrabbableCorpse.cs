using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public class GrabbableCorpse : IGrabbable
    {
        private readonly TTTPlayer _owner;
        private readonly PlayerCorpse _corpse;
        private PhysicsBody _handPhysicsBody;
        private readonly PhysicsBody _corpsePhysicsBody;
        private readonly int _corpseBone;
        private WeldJoint _holdJoint;

        public bool IsHolding
        {
            get => _holdJoint.IsValid;
        }

        public GrabbableCorpse(TTTPlayer player, PlayerCorpse corpse, PhysicsBody physicsBodyCorpse, int corpseBone)
        {
            _owner = player;
            _corpse = corpse;
            _corpsePhysicsBody = physicsBodyCorpse;
            _corpseBone = corpseBone;

            _handPhysicsBody = PhysicsWorld.AddBody();
            _handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;

            Transform attachment = player.GetAttachment(Hands.MIDDLE_HANDS_ATTACHMENT)!.Value;
            _handPhysicsBody.Position = attachment.Position;
            _handPhysicsBody.Rotation = attachment.Rotation;

            _holdJoint = PhysicsJoint.Weld
                .From(_handPhysicsBody)
                .To(physicsBodyCorpse)
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

            Transform attachment = player.GetAttachment(Hands.MIDDLE_HANDS_ATTACHMENT)!.Value;
            _handPhysicsBody.Position = attachment.Position;
            _handPhysicsBody.Rotation = attachment.Rotation;
        }

        public void SecondaryAction()
        {
            TraceResult tr = Trace.Ray(_owner.EyePos, _owner.EyePos + _owner.EyeRot.Forward * Hands.MAX_INTERACT_DISTANCE)
                .Ignore(_owner)
                .Run();

            if (!tr.Hit || !tr.Entity.IsValid())
            {
                _corpse.ClearAttachments();

                return;
            }

            Entity attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;

            if (!attachEnt.IsWorld)
            {
                _corpse.ClearAttachments();

                return;
            }

            Particles rope = Particles.Create("particles/rope.vpcf");
            rope.SetEntityBone(0, _corpsePhysicsBody.Entity, _corpseBone, new Transform(_corpsePhysicsBody.Transform.PointToLocal(_corpsePhysicsBody.Position) * (1.0f / _corpsePhysicsBody.Entity.Scale)));
            rope.SetPosition(1, tr.Body.Transform.PointToLocal(tr.EndPos));

            SpringJoint spring = PhysicsJoint.Spring
                    .From(_corpsePhysicsBody, _corpsePhysicsBody.Transform.PointToLocal(_corpsePhysicsBody.Position))
                    .To(tr.Body, tr.Body.Transform.PointToLocal(tr.EndPos))
                    .WithFrequency(1f)
                    .WithDampingRatio(1f)
                    .WithReferenceMass(_corpsePhysicsBody.PhysicsGroup.Mass)
                    .WithMinRestLength(0)
                    .WithMaxRestLength(10f)
                    .Create();

            _corpse.Ropes.Add(rope);
            _corpse.RopeSprings.Add(spring);

            Drop();
        }
    }
}
