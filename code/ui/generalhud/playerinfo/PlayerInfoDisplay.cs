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

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class PlayerInfoDisplay : Panel
    {
        private Panel _healthPanel;
        private Label _healthLabel;
        private Panel _creditPanel;
        private Label _creditLabel;

        public PlayerInfoDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/playerinfo/PlayerInfoDisplay.scss");

            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("opacity-heavy");
            AddClass("text-shadow");

            _healthPanel = new Panel(this);
            _creditPanel = new Panel(this);

            _healthLabel = _healthPanel.Add.Label();
            _healthLabel.AddClass("info");

            _creditLabel = _creditPanel.Add.Label();
            _creditLabel.AddClass("info");
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Enabled = Local.Pawn is TTTPlayer || (player.IsSpectator && player.IsSpectatingPlayer);

            _healthLabel.SetClass("hidden", player.CurrentPlayer.LifeState == LifeState.Alive);
            _healthLabel.Text = $"✚ {player.CurrentPlayer.Health.CeilToInt()}";

            _creditLabel.Text = $"$ {player.CurrentPlayer.Credits}";
        }
    }
}
