using Sandbox;

using TTTReborn.Globalization;

namespace TTTReborn.Events.Player
{
    [GameEvent("player_spawn"), Hammer.Skip]
    public partial class SpawnEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public TranslationData GetDescriptionTranslationData() => new(GetTranslationKey("DESCRIPTION"), PlayerName ?? "???");

        /// <summary>
        /// Occurs when a player spawns.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player spawned.</para>
        /// </summary>
        public SpawnEvent(TTTReborn.Player player) : base(player) { }

        public bool Contains(Client client) => Name == client.Name;
    }
}
