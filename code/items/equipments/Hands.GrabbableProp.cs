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
        public TTTPlayer Owner;

        public bool IsHolding
        {
            get => GrabbedEntity != null;
        }

        public GrabbableProp(TTTPlayer player, Entity ent)
        {
            Owner = player;

            GrabbedEntity = ent;

            if (GrabbedEntity is IPickupable pickupable)
            {
                pickupable.PickupTrigger.EnableTouch = false;
            }

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
                    pickupable.PickupTrigger.EnableTouch = true;
                }
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
                // Since the weapon now has a new owner/parent, no need to set parent to null.
                GrabbedEntity.EnableHideInFirstPerson = true;
                GrabbedEntity = null;

                return;
            }
        }

        public void SecondaryAction()
        {
            Owner.SetAnimParameter("b_attack", true);

            GrabbedEntity.SetParent(null);
            GrabbedEntity.EnableHideInFirstPerson = true;
            GrabbedEntity.Velocity += Owner.EyeRotation.Forward * THROW_FORCE;

            _ = WaitForAnimationFinish();
        }

        private async Task WaitForAnimationFinish()
        {
            try
            {
                await GameTask.DelaySeconds(0.6f);
            }
            catch (Exception e)
            {
                if (e.Message.Trim() != "A task was canceled.")
                {
                    Log.Error($"[TASK] {e.Message}: {e.StackTrace}");
                }
            }
            finally
            {
                GrabbedEntity = null;
            }
        }
    }
}
