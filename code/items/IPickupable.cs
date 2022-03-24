using Sandbox;

namespace TTTReborn.Items
{
    public interface IPickupable
    {
        PickupTrigger PickupTrigger { get; set; }
        Entity LastDropOwner { get; set; }
        TimeSince TimeSinceLastDrop { get; set; }

        void PickupStartTouch(Entity other);

        void PickupEndTouch(Entity other);
    }
}
