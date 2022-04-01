using System.Linq;
using System.Text.Json.Serialization;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class ClientGameEvent : NetworkableGameEvent
    {
        public int Ident { get; set; }

        public long PlayerId { get; set; }

        public string PlayerName { get; set; }

        [JsonIgnore]
        public Client Client
        {
            get => Client.All.First((cl) => cl.NetworkIdent == Ident);
        }

        public ClientGameEvent(Client client) : base()
        {
            if (client != null)
            {
                Ident = client.NetworkIdent;
                PlayerId = client.PlayerId;
                PlayerName = client.Name;
            }
        }

        public override void Run() => Event.Run(Name, Client);
    }
}
