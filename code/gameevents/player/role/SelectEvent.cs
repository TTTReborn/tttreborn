using Sandbox;

using TTTReborn.Globalization;

namespace TTTReborn.Events.Player.Role
{
    [GameEvent("player_role_select"), HideInEditor]
    public partial class SelectEvent : PlayerGameEvent, ILoggedGameEvent
    {
        public string RoleName { get; set; }

        public TranslationData GetDescriptionTranslationData() => new(GetTranslationKey("DESCRIPTION"), PlayerName ?? "???", RoleName != null ? new TranslationData(Utils.GetTranslationKey(RoleName, "NAME")) : "???");

        /// <summary>
        /// Occurs when a player selects their role.
        /// <para>Event is passed the <strong><see cref="TTTReborn.Player"/></strong> instance of the player whose role was set.</para>
        /// </summary>
        public SelectEvent(TTTReborn.Player player) : base(player)
        {
            if (player != null)
            {
                RoleName = player.Role.Name;
            }
        }

        public bool Contains(Client client) => PlayerName == client.Name;
    }
}
