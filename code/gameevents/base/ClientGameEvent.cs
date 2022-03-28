using System.Linq;

using Sandbox;

namespace TTTReborn.Events
{
    public partial class ClientGameEvent : ParameterlessGameEvent
    {
        [Net]
        public int Ident { get; set; }

        public Client Client
        {
            get => Client.All.First((cl) => cl.NetworkIdent == Ident);
        }

        public ClientGameEvent(Client client) : base()
        {
            Ident = client.NetworkIdent;
        }

        public override void Run() => Event.Run(Name, Client);
    }
}
