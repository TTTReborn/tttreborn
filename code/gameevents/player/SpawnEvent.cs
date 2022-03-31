namespace TTTReborn.Events.Player
{
    [GameEvent("player_spawn")]
    public partial class SpawnEvent : PlayerGameEvent
    {
        public override bool IsLogged { get; set; } = true;

        /// <summary>
        /// Occurs when a player spawns.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player spawned.</para>
        /// </summary>
        public SpawnEvent(TTTReborn.Player player) : base(player) { }
    }
}
