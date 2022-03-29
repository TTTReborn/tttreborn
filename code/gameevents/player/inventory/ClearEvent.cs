namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_clear")]
    public partial class ClearEvent : GameEvent
    {
        /// <summary>
        /// Occurs when the player's inventory is cleared.
        /// </summary>
        public ClearEvent() : base() { }
    }
}
