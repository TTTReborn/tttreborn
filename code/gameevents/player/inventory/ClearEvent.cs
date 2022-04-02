namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_clear"), Hammer.Skip]
    public partial class ClearEvent : NetworkableGameEvent
    {
        /// <summary>
        /// Occurs when the player's inventory is cleared.
        /// </summary>
        public ClearEvent() : base() { }
    }
}
