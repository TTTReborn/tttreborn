namespace TTTReborn.Events.Player.Role
{
    [GameEvent("player_role_select")]
    public partial class SelectEvent : PlayerGameEvent
    {
        public override bool IsLogged { get; set; } = true;

        /// <summary>
        /// Occurs when a player selects their role.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player whose role was set.</para>
        /// </summary>
        public SelectEvent(TTTReborn.Player player) : base(player) { }
    }
}
