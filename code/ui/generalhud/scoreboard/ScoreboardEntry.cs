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
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class ScoreboardEntry : Panel
    {
        public string ScoreboardGroupName;
        public Client Client;

        private Image _playerAvatar;
        private readonly Label _playerName;

        private readonly Label _karma;
        private readonly Label _ping;

        public ScoreboardEntry()
        {
            AddClass("text-shadow");
            AddClass("entry");

            _playerAvatar = Add.Image();
            _playerAvatar.AddClass("circular");
            _playerAvatar.AddClass("avatar");

            _playerName = Add.Label();
            _playerName.AddClass("name-label");

            _karma = Add.Label("", "karma");
            _ping = Add.Label("", "ping");
        }

        public virtual void Update()
        {
            if (Client == null)
            {
                return;
            }

            _playerName.Text = Client.Name;
            _karma.Text = Client.GetInt("karma").ToString();

            SetClass("me", Client == Local.Client);

            if (Client.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (player.Role is not NoneRole && player.Role is not InnocentRole)
            {
                Style.BackgroundColor = player.Role.Color.WithAlpha(0.15f);
            }
            else
            {
                Style.BackgroundColor = null;
            }

            _playerAvatar.SetTexture($"avatar:{Client.PlayerId}");
        }

        public override void Tick()
        {
            base.Tick();

            if (Client != null)
            {
                _ping.Text = Client.Ping.ToString();
            }
        }
    }
}
