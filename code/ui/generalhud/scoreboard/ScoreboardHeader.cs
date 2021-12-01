using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class ScoreboardHeader : Panel
    {
        private readonly Panel _gameLogoPanel;
        private readonly Panel _serverInfoPanel;
        private readonly TranslationLabel _serverNameLabel;
        private readonly TranslationLabel _serverDescriptionLabel;
        private readonly Panel _serverDataPanel;
        private readonly Label _serverMapLabel;
        private readonly TranslationLabel _serverPlayersLabel;

        public ScoreboardHeader(Sandbox.UI.Panel parent) : base(parent)
        {
            AddClass("text-shadow");

            _gameLogoPanel = new(this);
            _gameLogoPanel.AddClass("game-logo");

            _serverInfoPanel = new(this);
            _serverInfoPanel.AddClass("server-information-panel");

            _serverNameLabel = _serverInfoPanel.Add.TranslationLabel(new Globalization.TranslationData());
            _serverNameLabel.AddClass("server-name-label");
            _serverNameLabel.SetTranslation(new Globalization.TranslationData("SCOREBOARD_GAMEMODE", "Trouble in Terry's Town"));

            _serverDescriptionLabel = _serverInfoPanel.Add.TranslationLabel(new Globalization.TranslationData());
            _serverDescriptionLabel.AddClass("server-description-label");
            _serverDescriptionLabel.SetTranslation(new Globalization.TranslationData("SCOREBOARD_CREATEDBY", "Neoxult"));

            _serverDataPanel = new(this);
            _serverDataPanel.AddClass("server-data-panel");

            _serverMapLabel = _serverDataPanel.Add.Label();
            _serverMapLabel.AddClass("server-map-label");

            _serverPlayersLabel = _serverDataPanel.Add.TranslationLabel(new Globalization.TranslationData());
            _serverPlayersLabel.AddClass("server-players-label");

            UpdateServerInfo();
        }

        public void UpdateServerInfo()
        {
            int maxPlayers = ConsoleSystem.GetValue("maxplayers").ToInt(0);

            _serverMapLabel.Text = Global.MapName;
            _serverPlayersLabel.SetTranslation(new Globalization.TranslationData("SCOREBOARD_SERVER_PLAYERAMOUNT", Client.All.Count, maxPlayers));
        }
    }
}
