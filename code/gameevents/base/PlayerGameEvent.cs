using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class PlayerGameEvent : NetworkableGameEvent
    {
        public long PlayerId { get; set; }

        public string PlayerName { get; set; }

        [JsonIgnore]
        public TTTReborn.Player Player
        {
            get => Utils.GetPlayerById(PlayerId);
        }

        public PlayerGameEvent(TTTReborn.Player player) : base()
        {
            if (player != null && player.Client != null)
            {
                PlayerId = player.Client.PlayerId;
                PlayerName = player.Client.Name;
            }
        }

        public override void Run() => Event.Run(Name, Player);
    }
}
