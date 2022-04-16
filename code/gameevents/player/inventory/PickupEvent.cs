using Sandbox;

namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_pickup"), Hammer.Skip]
    public partial class PickupEvent : EntityGameEvent
    {
        /// <summary>
        /// Occurs when an item is picked up.
        /// <para>Event is passed the <strong><see cref="Entity"/></strong> instance of the item picked up.</para>
        /// </summary>
        public PickupEvent(Entity entity) : base(entity) { }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public PickupEvent() : base() { }
    }
}
