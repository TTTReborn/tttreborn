using Sandbox;

namespace TTTReborn;

public partial class Player
{
	/// <summary>
	/// Player's inventory for entities that can be carried. See <see cref="BaseCarriable"/>.
	/// </summary>
	public Inventory Inventory { get; protected set; }

    private static int CarriableDropVelocity { get; set; } = 300;

    private void TickPlayerDropCarriable()
    {
        using (Prediction.Off())
        {
            if (Input.Pressed(InputButton.Drop) && !Input.Down(InputButton.Run) && ActiveChild != null && Inventory != null)
            {
                Entity droppedEntity = Inventory.DropActive();

                if (droppedEntity != null)
                {
                    if (droppedEntity.PhysicsGroup != null)
                    {
                        droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * CarriableDropVelocity;
                    }
                }
            }
        }
    }

    private void TickItemSimulate()
    {
        if (Client == null)
        {
            return;
        }

        PerksInventory perks = Inventory.Perks;

        for (int i = 0; i < perks.Count(); i++)
        {
            perks.Get(i).Simulate(Client);
        }
    }
}
