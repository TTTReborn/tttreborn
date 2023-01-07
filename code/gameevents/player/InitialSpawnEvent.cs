using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_initialspawn"), HideInEditor]
    public partial class InitialSpawnEvent : ClientGameEvent
    {
        /// <summary>
        /// Occurs when a player initializes.
        /// <para>The <strong><see cref="IClient"/></strong> instance of the player who spawned initially.</para>
        /// </summary>
        public InitialSpawnEvent(IClient client) : base(client) { }
    }
}
