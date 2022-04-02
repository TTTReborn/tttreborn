namespace TTTReborn.Events.Player.Spectating
{
    [GameEvent("player_spectating_change"), Hammer.Skip]
    public partial class ChangeEvent : PlayerGameEvent
    {
        /// <summary>
        /// Occurs when the player is changed to spectate.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player who was changed to spectate.</para>
        /// </summary>
        public ChangeEvent(TTTReborn.Player player) : base(player) { }
    }
}
