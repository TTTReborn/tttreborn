using Sandbox;

namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_drop")]
    public partial class DropEvent : EntityGameEvent
    {
        /// <summary>
        /// Occurs when an item is dropped.
        /// <para>Event is passed the <strong><see cref="Entity"/></strong> instance of the item dropped.</para>
        /// </summary>
        public DropEvent(Entity entity) : base(entity) { }
    }
}
