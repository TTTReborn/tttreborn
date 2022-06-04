namespace TTTReborn.Events.Player.Inventory
{
    [GameEvent("player_inventory_clear"), HideInEditor]
    public partial class ClearEvent : NetworkableGameEvent
    {
        /// <summary>
        /// Occurs when the player's inventory is cleared.
        /// </summary>
        public ClearEvent() : base() { }
    }
}
