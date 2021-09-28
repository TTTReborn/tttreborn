namespace TTTReborn.Events
{
    public class PlayerEvents
    {
        /// <summary>
        /// Triggered when a player dies. <c>TTTPlayer</c> object is passed to events.
        /// </summary>
        public TTTEvents Died => new("tttreborn.player.died");

        /// <summary>
        /// Triggered when a player spawns. <c>TTTPlayer</c> object is passed to events.
        /// </summary>
        public TTTEvents Spawned => new("tttreborn.player.spawned");

        /// <summary>
        /// Events that are specifically triggered with role changes.
        /// </summary>
        public RoleEvents RoleEvents { get; } = new();

        /// <summary>
        /// Events that are specifically triggered with inventory changes.
        /// </summary>
        public InventoryEvents InventoryEvents { get; } = new();
    }
}
