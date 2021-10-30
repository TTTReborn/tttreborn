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

        public void SecondaryAction() { }
    }
}
