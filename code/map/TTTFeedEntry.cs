// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Teams;

namespace TTTReborn.Map
{
    [Library("ttt_feed_entry", Description = "Add text entry to the game feed when input fired.")]
    public partial class TTTFeedEntry : Entity
    {
        [Property("Message")]
        public string Message { get; set; } = "";

        [Property("Receiver", "Who will this message go to? If using a custom team, choose `Other` and set the `Receiver Team Override` to the name of your team.")]
        public FeedEntryType Receiver { get; set; } = FeedEntryType.Activator;

        [Property("Text color")]
        public Color Color { get; set; } = Color.White;

        [Property("Receiver Team Override")]
        public string ReceiverTeamOverride { get; set; } = "Override Team Name";

        [Input]
        public void DisplayMessage(Entity activator)
        {
            switch (Receiver)
            {
                case FeedEntryType.Activator:
                    RPCs.ClientDisplayMessage(To.Single(activator), Message, Color);

                    break;

                case FeedEntryType.All:
                    RPCs.ClientDisplayMessage(To.Everyone, Message, Color);

                    break;

                case FeedEntryType.Innocents:
                    RPCs.ClientDisplayMessage(To.Multiple(TeamFunctions.GetTeam(typeof(InnocentTeam)).GetClients()), Message, Color);

                    break;

                case FeedEntryType.Traitors:
                    RPCs.ClientDisplayMessage(To.Multiple(TeamFunctions.GetTeam(typeof(TraitorTeam)).GetClients()), Message, Color);

                    break;

                case FeedEntryType.Other:
                    TTTTeam team = TeamFunctions.TryGetTeam(ReceiverTeamOverride);

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

    public enum FeedEntryType
    {
        All,
        Activator,
        Innocents,
        Traitors,
        Other
    }
}
