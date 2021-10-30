using Sandbox;
using Sandbox.Joints;

using TTTReborn.Player;

namespace TTTReborn.Items
{

    public class GrabbableWeapon : IGrabbable
    {
        public TTTWeapon GrabbedEntity { get; set; }

        public bool IsHolding
        {
            get => GrabbedEntity != null;
        }

        public void Grab(TTTPlayer player, TraceResult tr)
        {
            GrabbedEntity = tr.Entity as TTTWeapon;
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
            // If this weapon gets pickedup by another player, we should drop it.
            if (GrabbedEntity.Owner != null)
            {
                Drop();
                return;
            }
        }

        public void SecondaryAction() { }
    }
}
