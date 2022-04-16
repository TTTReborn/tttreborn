using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_connected"), Hammer.Skip]
    public partial class ConnectedEvent : ClientGameEvent
    {
        /// <summary>
        /// Occurs when a player connects.
        /// <para>The <strong><see cref="Client"/></strong> instance of the player who connected.</para>
        /// </summary>
        public ConnectedEvent(Client client) : base(client) { }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public ConnectedEvent() : base() { }
    }
}
