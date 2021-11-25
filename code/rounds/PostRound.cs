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

using TTTReborn.Player;
using TTTReborn.Settings;

namespace TTTReborn.Rounds
{
    public class PostRound : BaseRound
    {
        public override string RoundName => "Post";
        public override int RoundDuration
        {
            get => ServerSettings.Instance.Round.PostRoundTime;
        }

        protected override void OnTimeUp()
        {
            base.OnTimeUp();

            // TODO: Allow users to close the menu themselves using mouse cursor.
            RPCs.ClientClosePostRoundMenu();

            bool shouldChangeMap = Gamemode.Game.Instance.MapSelection.TotalRoundsPlayed >= ServerSettings.Instance.Round.TotalRounds;
            Gamemode.Game.Instance.ChangeRound(shouldChangeMap ? new MapSelectionRound() : new PreRound());
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            player.MakeSpectator();
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                using (Prediction.Off())
                {
                    foreach (TTTPlayer player in Utils.GetPlayers())
                    {
                        if (player.PlayerCorpse != null && player.PlayerCorpse.IsValid() && player.LifeState == LifeState.Dead && !player.PlayerCorpse.IsIdentified)
                        {
                            player.PlayerCorpse.IsIdentified = true;

                            RPCs.ClientConfirmPlayer(null, player.PlayerCorpse, player, player.Role.Name, player.Team.Name, player.PlayerCorpse.GetConfirmationData(), player.PlayerCorpse.KillerWeapon, player.PlayerCorpse.Perks);
                        }
                        else
                        {
                            player.SendClientRole(To.Everyone);
                        }
                    }
                }
            }
        }
    }
}
