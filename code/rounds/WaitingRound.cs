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

using System.Threading.Tasks;
using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Rounds
{
    public class WaitingRound : BaseRound
    {
        public override string RoundName => "Waiting";

        public override void OnSecond()
        {
            if (Host.IsServer && Utils.HasMinimumPlayers())
            {
                Gamemode.Game.Instance.ForceRoundChange(new PreRound());
            }
        }

        public override void OnPlayerKilled(TTTPlayer player)
        {
            StartRespawnTimer(player);

            player.MakeSpectator();

            base.OnPlayerKilled(player);
        }

        protected override void OnStart()
        {
            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer player)
                    {
                        player.Respawn();
                    }
                }
            }
        }

        private static async void StartRespawnTimer(TTTPlayer player)
        {
            try
            {
                await GameTask.DelaySeconds(1);

                if (player.IsValid() && Gamemode.Game.Instance.Round is WaitingRound)
                {
                    player.Respawn();
                }
            }
            catch (Exception e)
            {
                if (e.Message.Trim() == "A task was canceled.")
                {
                    return;
                }

                Log.Error($"[TASK] {e.Message}: {e.StackTrace}");
            }
        }
    }
}
