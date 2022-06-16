using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_connected"), HideInEditor]
    public partial class ConnectedEvent : ClientGameEvent
    {
        /// <summary>
        /// Occurs when a player connects.
        /// <para>The <strong><see cref="Client"/></strong> instance of the player who connected.</para>
        /// </summary>
        public ConnectedEvent(Client client) : base(client) { }
    }
}
