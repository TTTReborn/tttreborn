using System.Collections.Generic;

using Sandbox;

namespace TTTReborn;

public class BaseInventory : IBaseInventory
{
	public Entity Owner { get; init; }
	public List<Entity> List = new();
	public virtual Entity Active
	{
		get
		{
			return (Owner as Player)?.ActiveChild;
		}

		set
		{
			if (Owner is Player player)
			{
				player.ActiveChild = value;
			}
		}
	}


	public BaseInventory(Entity owner)
	{
		Owner = owner;
	}

	/// <summary>
	/// Return true if this item belongs in the inventory
	/// </summary>
	public virtual bool CanAdd(Entity ent) => ent is BaseCarriable bc && bc.CanCarry(Owner);

	/// <summary>
	/// Delete every entity we're carrying. Useful to call on death.
	/// </summary>
	public virtual void DeleteContents()
	{
		Game.AssertServer();

		foreach (Entity item in List.ToArray())
		{
			item.Delete();
		}

		List.Clear();
	}

	/// <summary>
	/// Get the item in this slot
	/// </summary>
	public virtual Entity GetSlot(int i)
	{
		if (List.Count <= i || i < 0)
        {
            return null;
        }

		return List[i];
	}

	/// <summary>
	/// Returns the number of items in the inventory
	/// </summary>
	public virtual int Count() => List.Count;

	/// <summary>
	/// Returns the index of the currently active child
	/// </summary>
	public virtual int GetActiveSlot()
	{
		Entity ae = Active;
		int count = Count();

		for (int i = 0; i < count; i++)
		{
			if (List[i] == ae)
            {
				return i;
            }
		}

		return -1;
	}

	/// <summary>
	/// Try to pick this entity up
	/// </summary>
	public virtual void Pickup(Entity ent) { }

	/// <summary>
	/// A child has been added to the Owner (player). Do we want this
	/// entity in our inventory? Yeah? Add it then.
	/// </summary>
	public virtual void OnChildAdded(Entity child)
	{
		if (!CanAdd(child))
        {
			return;
        }

		if (List.Contains(child))
        {
			throw new System.Exception( "Trying to add to inventory multiple times. This is gated by Entity:OnChildAdded and should never happen!" );
        }

		List.Add(child);
	}

	/// <summary>
	/// A child has been removed from our Owner. This might not even
	/// be in our inventory, if it is then we'll remove it from our list
	/// </summary>
	public virtual void OnChildRemoved(Entity child)
	{
		if (List.Remove(child))
		{
			// On removed etc
		}
	}

	/// <summary>
	/// Set our active entity to the entity on this slot
	/// </summary>
	public virtual bool SetActiveSlot(int i, bool evenIfEmpty = false)
	{
		Entity ent = GetSlot(i);

		if (Active == ent || !evenIfEmpty && ent == null)
		{
        	return false;
        }

		Active = ent;

		return ent.IsValid();
	}

	/// <summary>
	/// Switch to the slot next to the slot we have active.
	/// </summary>
	public virtual bool SwitchActiveSlot(int idelta, bool loop)
	{
		int count = Count();

		if (count == 0)
        {
            return false;
        }

		int slot = GetActiveSlot();
		int nextSlot = slot + idelta;

		if (loop)
		{
			while (nextSlot < 0)
            {
                nextSlot += count;
            }
			while (nextSlot >= count)
            {
                nextSlot -= count;
            }
		}
		else
		{
			if (nextSlot < 0 || nextSlot >= count)
            {
                return false;
            }
		}

		return SetActiveSlot(nextSlot, false);
	}

	/// <summary>
	/// Drop the active entity. If we can't drop it, will return null
	/// </summary>
	public virtual Entity DropActive()
	{
		if (!Game.IsServer)
        {
            return null;
        }

		Entity ac = Active;

		if (ac == null)
        {
            return null;
        }

		if (Drop(ac))
		{
			Active = null;

			return ac;
		}

		return null;
	}

	/// <summary>
	/// Drop this entity. Will return true if successfully dropped.
	/// </summary>
	public virtual bool Drop(Entity ent)
	{
		if (!Game.IsServer || !Contains(ent))
        {
			return false;
        }

		ent.Parent = null;

		if (ent is BaseCarriable bc)
		{
			bc.OnCarryDrop(Owner);
		}

		return true;
	}

	/// <summary>
	/// Returns true if this inventory contains this entity
	/// </summary>
	public virtual bool Contains(Entity ent) => List.Contains(ent);


	/// <summary>
	/// Make this entity the active one
	/// </summary>
	public virtual bool SetActive(Entity ent)
	{
		if (Active == ent || !Contains(ent))
        {
            return false;
        }

		Active = ent;

		return true;
	}

	/// <summary>
	/// Try to add this entity to the inventory. Will return true
	/// if the entity was added successfully.
	/// </summary>
	public virtual bool Add(Entity ent, bool makeActive = false)
	{
		Game.AssertServer();

		//
		// Can't pickup if already owned
		//
		if (ent.Owner != null)
        {
            return false;
        }

		//
		// Let the inventory reject the entity
		//
		if (!CanAdd(ent))
        {
			return false;
        }

		if (ent is not BaseCarriable carriable)
        {
			return false;
        }

		//
		// Let the entity reject the inventory
		//
		if (!carriable.CanCarry(Owner))
        {
			return false;
        }

		//
		// Passed!
		//

		ent.Parent = Owner;

		//
		// Let the item do shit
		//
		carriable.OnCarryStart(Owner);

		if (makeActive)
		{
			SetActive(ent);
		}

		return true;
	}
}
