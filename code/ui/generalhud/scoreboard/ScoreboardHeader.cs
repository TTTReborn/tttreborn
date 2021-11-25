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

            _serverNameLabel = _serverInfoPanel.Add.TranslationLabel();
            _serverNameLabel.AddClass("server-name-label");
            _serverNameLabel.SetTranslation("SCOREBOARD_GAMEMODE", "Trouble in Terry's Town");

            _serverDescriptionLabel = _serverInfoPanel.Add.TranslationLabel();
            _serverDescriptionLabel.AddClass("server-description-label");
            _serverDescriptionLabel.SetTranslation("SCOREBOARD_CREATEDBY", "Neoxult");

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
