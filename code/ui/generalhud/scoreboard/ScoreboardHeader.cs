using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class ScoreboardHeader : Panel
    {
        private readonly Panel _gameLogoPanel;
        private readonly Panel _serverInfoPanel;
        private readonly Label _serverNameLabel;
        private readonly Label _serverDescriptionLabel;
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

            _serverNameLabel = _serverInfoPanel.Add.Label();
            _serverNameLabel.AddClass("server-name-label");
            _serverNameLabel.Text = "Trouble in Terry's Town Server";

            _serverDescriptionLabel = _serverInfoPanel.Add.Label();
            _serverDescriptionLabel.AddClass("server-description-label");
            _serverDescriptionLabel.Text = "Created by Neoxult";

            _serverDataPanel = new(this);
            _serverDataPanel.AddClass("server-data-panel");

            _serverMapLabel = _serverDataPanel.Add.Label();
            _serverMapLabel.AddClass("server-map-label");

            _serverPlayersLabel = _serverDataPanel.Add.TranslationLabel();
            _serverPlayersLabel.AddClass("server-players-label");

            UpdateServerInfo();
        }

        public void UpdateServerInfo()
        {
            int maxPlayers = ConsoleSystem.GetValue("maxplayers").ToInt(0);

            _serverMapLabel.Text = Global.MapName;
            _serverPlayersLabel.SetTranslation("SCOREBOARD_SERVER_PLAYERAMOUNT", Client.All.Count, maxPlayers);
        }
    }
}
