using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public class GrabbableCorpse : IGrabbable
    {
        private PhysicsBody _handPhysicsBody;
        private WeldJoint _holdJoint;

        public bool IsHolding
        {
            get => _holdJoint.IsValid;
        }

        public GrabbableCorpse()
        {
            _handPhysicsBody = PhysicsWorld.AddBody();
            _handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;
        }

        public void Grab(TTTPlayer player, TraceResult tr)
        {
            Transform attachment = player.GetAttachment(IGrabbable.MIDDLE_HANDS_ATTACHMENT)!.Value;
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

        public void SecondaryAction()
        {
            TraceResult tr = Trace.Ray(Owner.EyePos, Owner.EyePos + Owner.EyeRot.Forward * _grabbingDistance)
                .Ignore(Owner)
                .Run();

            if (!tr.Hit || !tr.Entity.IsValid())
            {
                playerCorpse.ClearAttachments();

                return;
            }

            Entity attachEnt = tr.Body.IsValid() ? tr.Body.Entity : tr.Entity;

            if (!attachEnt.IsWorld)
            {
                playerCorpse.ClearAttachments();

                return;
            }

            Particles rope = Particles.Create("particles/rope.vpcf");
            rope.SetEntityBone(0, _heldBody.Entity, _heldBone, new Transform(_heldBody.Transform.PointToLocal(_heldBody.Position) * (1.0f / _heldBody.Entity.Scale)));
            rope.SetPosition(1, tr.Body.Transform.PointToLocal(tr.EndPos));

            SpringJoint spring = PhysicsJoint.Spring
                    .From(_heldBody, _heldBody.Transform.PointToLocal(_heldBody.Position))
                    .To(tr.Body, tr.Body.Transform.PointToLocal(tr.EndPos))
                    .WithFrequency(1f)
                    .WithDampingRatio(1f)
                    .WithReferenceMass(_heldBody.PhysicsGroup.Mass)
                    .WithMinRestLength(0)
                    .WithMaxRestLength(10f)
                    .Create();

            playerCorpse.Ropes.Add(rope);
            playerCorpse.RopeSprings.Add(spring);
        }
    }
}
