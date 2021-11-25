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
using Sandbox.UI.Construct;

using TTTReborn.Events;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public class PlayerRoleDisplay : Panel
    {
        private TranslationLabel _roleLabel;

        public PlayerRoleDisplay() : base()
        {
            StyleSheet.Load("/ui/generalhud/playerinfo/PlayerRoleDisplay.scss");

            AddClass("rounded");
            AddClass("centered-horizontal");
            AddClass("opacity-heavy");
            AddClass("text-shadow");

            _roleLabel = Add.TranslationLabel("");
            _roleLabel.AddClass("centered");
            _roleLabel.AddClass("role-label");

            OnRoleUpdate(Local.Pawn as TTTPlayer);
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            Enabled = !player.IsSpectator && !player.IsSpectatingPlayer && Gamemode.Game.Instance.Round is Rounds.InProgressRound;
        }

        [Event(TTTEvent.Player.Role.Select)]
        private void OnRoleUpdate(TTTPlayer player)
        {
            if (player == null || player != Local.Pawn as TTTPlayer)
            {
                return;
            }

            Style.BackgroundColor = player.Role.Color;

            _roleLabel.SetTranslation(player.Role.GetRoleTranslationKey("NAME"));
        }
    }
}
