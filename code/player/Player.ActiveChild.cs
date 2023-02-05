using Sandbox;

namespace TTTReborn;

public partial class Player
{
	[Net, Predicted] public Entity ActiveChild { get; set; }
	[ClientInput] public Entity ActiveChildInput { get; set; }

	/// <summary>
	/// This isn't networked, but it's predicted. If it wasn't then when the prediction system
	/// re-ran the commands LastActiveChild would be the value set in a future tick, so ActiveEnd
	/// and ActiveStart would get called multiple times and out of order, causing all kinds of pain.
	/// </summary>
	[Predicted]
	public Entity LastActiveChild { get; set; }

	/// <summary>
	/// Simulated the active child. This is important because it calls ActiveEnd and ActiveStart.
	/// If you don't call these things, viewmodels and stuff won't work, because the entity won't
	/// know it's become the active entity.
	/// </summary>
	public virtual void SimulateActiveChild(IClient cl, Entity child)
	{
		if (LastActiveChild != child)
		{
			OnActiveChildChanged(LastActiveChild, child);

			LastActiveChild = child;
		}

		if (!LastActiveChild.IsValid())
        {
			return;
        }

		if (LastActiveChild.IsAuthority)
		{
			LastActiveChild.Simulate(cl);
		}
	}

	/// <summary>
	/// Called when the Active child is detected to have changed
	/// </summary>
	public virtual void OnActiveChildChanged(Entity previous, Entity next)
	{
		if (previous is BaseCarriable previousBc)
		{
			previousBc?.ActiveEnd(this, previousBc.Owner != this);
		}

		if (next is BaseCarriable nextBc)
		{
			nextBc?.ActiveStart(this);
		}
	}

	public override void OnChildAdded(Entity child)
	{
		Inventory?.OnChildAdded(child);
	}

	public override void OnChildRemoved(Entity child)
	{
		Inventory?.OnChildRemoved(child);
	}
}
