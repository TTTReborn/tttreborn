namespace TTTReborn.Events
{
    public class InventoryEvents
    {
        /// <summary>
        /// Triggered when a item is picked up. <c>ICarriableItem</c> of the object that is picked up is passed to events.
        /// </summary>
        public TTTEvents OnItemPickUp => new("tttreborn.player.inventory.pickup");

        /// <summary>
        /// Triggered when a item is picked up. <c>ICarriableItem</c> of the object that is dropped is passed to events.
        /// </summary>
        public TTTEvents OnItemDropped => new("tttreborn.player.inventory.drop");

        /// <summary>
        /// Triggered when the player's inventory is cleared. <strong>No</strong> data is passed to this event.
        /// </summary>
        public TTTEvents OnClear => new("tttreborn.player.inventory.clear");
    }
}
