using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class PlayerGameEvent : GameEvent
    {
        public int Ident { get; set; }

        [JsonIgnore]
        public TTTReborn.Player Player
        {
            get => Utils.GetPlayerByIdent(Ident);
        }

        public PlayerGameEvent(TTTReborn.Player player) : base()
        {
            if (player != null)
            {
                Ident = player.NetworkIdent;
            }
        }

        public override void Run() => Event.Run(Name, Player);
    }
}
