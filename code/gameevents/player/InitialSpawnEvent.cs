using Sandbox;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_initialspawn"), Hammer.Skip]
    public partial class InitialSpawnEvent : ClientGameEvent
    {
        /// <summary>
        /// Occurs when a player initializes.
        /// <para>The <strong><see cref="Client"/></strong> instance of the player who spawned initially.</para>
        /// </summary>
        public InitialSpawnEvent(Client client) : base(client) { }

        /// <summary>
        /// WARNING! Do not use this constructor on your own! Used internally and is publicly visible due to sbox's `Library` library
        /// </summary>
        public InitialSpawnEvent() : base() { }
    }
}
