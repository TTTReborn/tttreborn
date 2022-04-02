using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class PlayerGameEvent : NetworkableGameEvent
    {
        public int Ident { get; set; }

        public long PlayerId { get; set; }

        public string PlayerName { get; set; }

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

                if (player.Client != null)
                {
                    PlayerId = player.Client.PlayerId;
                    PlayerName = player.Client.Name;
                }
            }
        }

        public override void Run() => Event.Run(Name, Player);
    }
}
