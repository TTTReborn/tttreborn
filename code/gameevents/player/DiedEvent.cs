namespace TTTReborn.Events.Player
{
    [GameEvent("player_died")]
    public partial class DiedEvent : PlayerGameEvent
    {
        /// <summary>
        /// Occurs when a player dies.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player who died.</para>
        /// </summary>
        public DiedEvent(TTTReborn.Player player) : base(player) { }
    }
}
