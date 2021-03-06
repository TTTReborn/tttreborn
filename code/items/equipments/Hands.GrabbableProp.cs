using Sandbox;

namespace TTTReborn.Items
{
    public class GrabbableProp : IGrabbable
    {
        public const float THROW_FORCE = 500;
        public Entity GrabbedEntity { get; set; }
        public Player Owner;

        public bool IsHolding
        {
            get => GrabbedEntity != null;
        }

        public GrabbableProp(Player player, Entity ent)
        {
            Owner = player;

            GrabbedEntity = ent;

            if (GrabbedEntity is IPickupable pickupable)
            {
                pickupable.PickupTrigger.EnableTouch = false;
            }

            // TODO You can drop entities with this through walls and the ground
            GrabbedEntity.SetParent(player, Hands.MIDDLE_HANDS_ATTACHMENT, new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
            GrabbedEntity.EnableHideInFirstPerson = false;
        }

        public void Drop()
        {
            if (GrabbedEntity?.IsValid ?? false)
            {
                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity.SetParent(null);

                if (GrabbedEntity is IPickupable pickupable)
                {
                    pickupable.LastDropOwner = Owner;
                    pickupable.TimeSinceLastDrop = 0f;
                    pickupable.PickupTrigger.EnableTouch = true;
                }
            }

            GrabbedEntity = null;
        }

        public void Update(Player player)
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
                // Since the weapon now has a new owner/parent, no need to set parent to null.
                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity = null;

                return;
            }
        }

        public void PrimaryAction()
        {
            if (GrabbedEntity?.IsValid ?? false)
            {
                Owner.SetAnimParameter("b_attack", true);

                Entity grabbedEntity = GrabbedEntity;

                Drop();

                grabbedEntity.Velocity += Owner.EyeRotation.Forward * THROW_FORCE;
            }
        }
    }
}
