using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{

    public class GrabbableProp : IGrabbable
    {
        public Entity GrabbedEntity { get; set; }

        public bool IsHolding
        {
            get => GrabbedEntity != null;
        }

        public GrabbableProp(TTTPlayer player, Entity ent)
        {
            GrabbedEntity = ent;
            GrabbedEntity.SetParent(player, IGrabbable.MIDDLE_HANDS_ATTACHMENT, new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
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

            // // If the entity gets another owner (i.e weapon pickup) drop it.
            if (GrabbedEntity?.Owner != null)
            {
                Drop();
                return;
            }
        }

        public void SecondaryAction() { }
    }
}
