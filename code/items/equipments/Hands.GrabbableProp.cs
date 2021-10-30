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

        public void Grab(TTTPlayer player, TraceResult tr)
        {
            GrabbedEntity = tr.Entity;
            GrabbedEntity.SetParent(player, IGrabbable.MIDDLE_HANDS_ATTACHMENT, new Transform(Vector3.Zero, Rotation.FromRoll(-90)));
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
            // If the prop gets destroyed.
            if (GrabbedEntity.Health == 0)
            {
                Drop();
                return;
            }

            // If the entity gets another owner (i.e weapon pickup)
            if (GrabbedEntity.Owner != null)
            {
                Drop();
                return;
            }
        }

        public void SecondaryAction() { }
    }
}
