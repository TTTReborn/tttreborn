using Sandbox;

namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_drop"), Hammer.Skip]
    public partial class DropEvent : EntityGameEvent
    {
        /// <summary>
        /// Occurs when an item is dropped.
        /// <para>Event is passed the <strong><see cref="Entity"/></strong> instance of the item dropped.</para>
        /// </summary>
        public DropEvent(Entity entity) : base(entity) { }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public DropEvent() : base() { }
    }
}
