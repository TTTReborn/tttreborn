using Sandbox;

namespace TTTReborn.Items
{
    public interface IPickupable
    {
        PickupTrigger PickupTrigger { get; set; }
        Entity LastDropOwner { get; set; }
        TimeSince SinceLastDrop { get; set; }

        void PickupStartTouch(Entity other);

        void PickupEndTouch(Entity other);
    }
}
