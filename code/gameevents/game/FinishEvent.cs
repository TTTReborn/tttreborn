using System.Text.Json.Serialization;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Teams;

namespace TTTReborn.Events.Game
{
    [GameEvent("game_finish"), Hammer.Skip]
    public partial class FinishEvent : NetworkableGameEvent, ILoggedGameEvent
    {
        public string TeamName { get; set; }

        [JsonIgnore]
        public Team Team
        {
            get => Utils.GetObjectByType<Team>(Utils.GetTypeByLibraryName<Team>(TeamName));
        }

        /// <summary>
        /// Round end event reporting the winning team.
        /// </summary>
        public FinishEvent(Team team) : base()
        {
            if (team != null)
            {
                TeamName = team.Name;
            }
        }

        public TranslationData GetDescriptionTranslationData() => new(GetTranslationKey("DESCRIPTION"), Team != null ? new TranslationData(Team.GetTranslationKey("NAME")) : "???");

        public override void Run() => Event.Run(Name, Team);

        public bool Contains(Client client) => true;
    }
}
