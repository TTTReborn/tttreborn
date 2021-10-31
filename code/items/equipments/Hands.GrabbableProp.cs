using System;
using System.Threading.Tasks;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public class GrabbableProp : IGrabbable
    {
        public const float THROW_FORCE = 500;
        public Entity GrabbedEntity { get; set; }
        public TTTPlayer _owner;

        public bool IsHolding
        {
            get => GrabbedEntity != null;
        }

        public GrabbableProp(TTTPlayer player, Entity ent)
        {
            _owner = player;

            GrabbedEntity = ent;
            GrabbedEntity.SetParent(player, Hands.MIDDLE_HANDS_ATTACHMENT, new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
            GrabbedEntity.EnableHideInFirstPerson = false;
        }

        public void Drop()
        {
            if (GrabbedEntity?.IsValid ?? false)
            {
                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity.SetParent(null);
            }

            GrabbedEntity = null;
        }

        public void Update(TTTPlayer player)
        {
            // If the entity is destroyed drop it.
            if (!GrabbedEntity?.IsValid ?? true)
            {
                Drop();

                return;
            }

            // If the entity gets another owner (i.e weapon pickup) drop it.
            if (GrabbedEntity?.Owner != null)
            {
                Drop();

                return;
            }
        }

        public void SecondaryAction()
        {
            _owner.SetAnimBool("b_attack", true);

            GrabbedEntity.SetParent(null);
            GrabbedEntity.EnableHideInFirstPerson = true;
            GrabbedEntity.Velocity += _owner.EyeRot.Forward * THROW_FORCE;

            _ = WaitForAnimationFinish();
        }

        private async Task WaitForAnimationFinish()
        {
            await GameTask.DelaySeconds(0.6f);
            GrabbedEntity = null;
        }
    }
}
