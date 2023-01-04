using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

#pragma warning disable IDE0052

namespace TTTReborn.UI
{
    [UseTemplate]
    public class ScoreboardHeader : Panel
    {
        private string ServerMapName { get; set; }
        private TranslationLabel ServerNameLabel { get; set; }
        private TranslationLabel ServerDescriptionLabel { get; set; }
        private TranslationLabel ServerPlayersLabel { get; set; }

        public ScoreboardHeader()
        {
            ServerNameLabel.UpdateTranslation(new TranslationData("SCOREBOARD.HEADER.GAMEMODE", "Trouble in Terry's Town"));
            ServerDescriptionLabel.UpdateTranslation(new TranslationData("SCOREBOARD.HEADER.CREATEDBY", "Neoxult"));

            UpdateServerInfo();
        }

        public void UpdateServerInfo()
        {
            ServerMapName = Game.Server.MapIdent ;
            ServerPlayersLabel.UpdateTranslation(new TranslationData("SCOREBOARD.HEADER.PLAYERAMOUNT", Game.Clients.Count, ConsoleSystem.GetValue("maxplayers").ToInt(0)));
        }
    }
}
