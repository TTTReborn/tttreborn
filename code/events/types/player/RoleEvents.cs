namespace TTTReborn.Events
{
    public class RoleEvents
    {
        /// <summary>
        /// Triggered when the player's inventory is cleared. <strong>No</strong> data is passed to this event.
        /// </summary>
        public TTTEvents OnSelect => new("tttreborn.player.role.onselect");
    }
}
