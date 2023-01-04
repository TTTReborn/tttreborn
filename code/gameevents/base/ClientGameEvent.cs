using System.Linq;
using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class ClientGameEvent : NetworkableGameEvent
    {
        public long PlayerId { get; set; }

        public string PlayerName { get; set; }

        [JsonIgnore]
        public IClient Client
        {
            get => Sandbox.Game.Clients.First((cl) => cl.SteamId == PlayerId);
        }

        public ClientGameEvent(IClient client) : base()
        {
            if (client != null)
            {
                PlayerId = client.SteamId;
                PlayerName = client.Name;
            }
        }

        public override void Run() => Event.Run(Name, Client);
    }
}
