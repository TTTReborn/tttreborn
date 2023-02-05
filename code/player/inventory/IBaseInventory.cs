using Sandbox;

namespace TTTReborn
{
	public interface IBaseInventory
	{
		void OnChildAdded(Entity child);
		void OnChildRemoved(Entity child);
		void DeleteContents();
		int Count();
		Entity GetSlot(int i);
		int GetActiveSlot();
		bool SetActiveSlot(int i, bool allowempty);
		bool SwitchActiveSlot(int idelta, bool loop);
		Entity DropActive();
		bool Drop(Entity ent);
		Entity Active { get; }
		bool SetActive(Entity ent);
		bool Add(Entity ent, bool makeactive = false);
		bool Contains(Entity ent);
	}
}
