using Sandbox;


using TTTReborn.Globals;
using TTTReborn.Teams;

namespace TTTReborn.MapEntities
{
    [Library("ttt_feed_entry", Description = "Add text entry to the game feed when input fired.")]
    public partial class TTTFeedEntry : Entity
    {
        [Property("Message")]
        public string Message { get; set; } = "";

        [Property("Receiver", "The name of the team receiving the message. `Activator` for activator of entity trigger, `All` for everyone, `Innocents` and `Traitors`.")]
        public string Receiver { get; set; } = "Activator";

        [Property("Text color")]
        public Color Color { get; set; } = Color.White;

        [Input]
        public void DisplayMessage(Entity activator)
        {
            switch (Receiver.ToLower())
            {
                case "activator":
                    RPCs.ClientDisplayMessage(To.Single(activator), Message, Color);
                    break;
                case "all":
                    RPCs.ClientDisplayMessage(To.Everyone, Message, Color);
                    break;
                case "innocents":
                    RPCs.ClientDisplayMessage(To.Multiple(TeamFunctions.GetTeam("Innocents").GetClients()), Message, Color);
                    break;
                case "traitors":
                    RPCs.ClientDisplayMessage(To.Multiple(TeamFunctions.GetTeam("Traitors").GetClients()), Message, Color);
                    break;
                default:
                    TTTTeam team = TeamFunctions.GetTeam(Receiver);

                    if (team != null)
                    {
                        RPCs.ClientDisplayMessage(To.Multiple(team.GetClients()), Message, Color);
                    }
                    else
                    {
                        Log.Warning($"Feed entry receiver value `{Receiver}` is incorrect and will not work.");
                    }
                    break;
            }
        }
    }
}
