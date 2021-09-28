namespace TTTReborn.Events
{
    public class PlayerEvents
    {
        /// <summary>
        /// Triggered when a player spawns. <c>TTTPlayer</c> object is passed to events.
        /// </summary>
        public TTTEvents Spawned => new("tttreborn.player.spawned");

        /// <summary>
        /// Triggered when a player dies. <c>TTTPlayer</c> object is passed to events.
        /// </summary>
        public TTTEvents Died => new("tttreborn.player.died");

        /// <summary>
        /// Triggered when the player's inventory is cleared. <strong>TTTPlayer</strong> is passed to this event.
        /// </summary>
        public TTTEvents OnRoleSelected => new("tttreborn.player.role.onselect");

        /// <summary>
        /// Events that are specifically triggered with inventory changes.
        /// </summary>
        public InventoryEvents Inventory { get; } = new();
    }
}
