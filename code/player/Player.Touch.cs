using Sandbox;

using TTTReborn.Items;

namespace TTTReborn;

public partial class Player
{
	public override void StartTouch(Entity other)
	{
		if (Game.IsClient)
        {
            return;
        }

        if (other is PickupTrigger && other.Parent is IPickupable pickupable)
        {
            pickupable.PickupStartTouch(this);
        }
	}

    public override void EndTouch(Entity other)
    {
        if (Game.IsClient)
        {
            return;
        }

        if (other is PickupTrigger && other.Parent is IPickupable pickupable)
        {
            pickupable.PickupEndTouch(this);
        }
    }
}
