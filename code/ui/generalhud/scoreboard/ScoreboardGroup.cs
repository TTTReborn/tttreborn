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
    public partial class ScoreboardGroup : Panel
    {
        public readonly string GroupTitle;
        public int GroupMembers = 0;

        private readonly Panel _groupTitlePanel;
        private readonly TranslationLabel _groupTitleLabel;
        private readonly TranslationLabel _groupKarmaLabel;
        private readonly TranslationLabel _groupPingLabel;
        private readonly Panel _groupContent;

        public ScoreboardGroup(Sandbox.UI.Panel parent, string groupName) : base(parent)
        {
            GroupTitle = groupName;

            AddClass(groupName);
            AddClass("text-shadow");

            _groupTitlePanel = new(this);
            _groupTitlePanel.AddClass("group-title-panel");
            _groupTitlePanel.AddClass("opacity-medium");
            _groupTitlePanel.AddClass("rounded-top");

            _groupTitleLabel = _groupTitlePanel.Add.TranslationLabel("");
            _groupTitleLabel.AddClass("group-title-label");

            _groupKarmaLabel = _groupTitlePanel.Add.TranslationLabel("SCOREBOARD_PLAYER_STATUS_KARMA");
            _groupKarmaLabel.AddClass("group-karma-label");

            _groupPingLabel = _groupTitlePanel.Add.TranslationLabel("SCOREBOARD_PLAYER_STATUS_PING");
            _groupPingLabel.AddClass("group-ping-label");

            _groupContent = new(this);
            _groupContent.AddClass("group-content-panel");
        }

        public ScoreboardEntry AddEntry(Client client)
        {
            ScoreboardEntry scoreboardEntry = _groupContent.AddChild<ScoreboardEntry>();
            scoreboardEntry.ScoreboardGroupName = GroupTitle;
            scoreboardEntry.Client = client;

            scoreboardEntry.Update();

            return scoreboardEntry;
        }

        // TODO: Implement logic for the player counter in the title
        public void UpdateLabel()
        {
            _groupTitleLabel.SetTranslation($"SCOREBOARD_GROUP_{GroupTitle.ToUpper()}", GroupMembers);
        }
    }
}
