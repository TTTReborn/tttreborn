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

using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;
using TTTReborn.Rounds;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private static TTTPlayer GetPlayerById(int id)
        {
            List<Client> playerList = Client.All.ToList();

            if (playerList.Count <= id)
            {
                return null;
            }

            if (playerList[id].Pawn is TTTPlayer player && player.IsValid())
            {
                return player;
            }

            return null;
        }

        [ServerCmd(Name = "ttt_respawn", Help = "Respawns the current player or the player with the given id")]
        public static void RespawnPlayer(string id = null)
        {
            if (!ConsoleSystem.Caller.HasPermission("respawn"))
            {
                return;
            }

            TTTPlayer player = null;

            if (id == null)
            {
                player = ConsoleSystem.Caller.Pawn as TTTPlayer;
            }
            else
            {
                try
                {
                    player = GetPlayerById(int.Parse(id));
                }
                catch (Exception)
                {
                    return;
                }
            }

            if (player == null || player.Client.GetValue<bool>("forcedspectator", false))
            {
                if (id == null)
                {
                    Log.Info($"You tried to respawn yourself while you've been a forced spectator this round.");
                }
                else
                {
                    Log.Info($"You tried to spawn the player '{player.Client.Name}' who have been a forced spectator this round.");
                }

                return;
            }

            player.Respawn();

            if (id == null)
            {
                Log.Info($"You respawned yourself.");
            }
            else
            {
                Log.Info($"You've respawned the player '{player.Client.Name}'.");
            }
        }

        [ServerCmd(Name = "ttt_requestitem")]
        public static void RequestItem(string itemName)
        {
            if (itemName == null)
            {
                return;
            }

            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid())
            {
                return;
            }

            Type itemType = Utils.GetTypeByLibraryName<IItem>(itemName);

            if (itemType == null || !Utils.HasAttribute<BuyableAttribute>(itemType))
            {
                return;
            }

            player.RequestPurchase(itemType);
        }

        [ServerCmd(Name = "ttt_setrole")]
        public static void SetRole(string roleName, string id = null)
        {
            if (!ConsoleSystem.Caller.HasPermission("role"))
            {
                return;
            }

            if (Gamemode.Game.Instance.Round is not Rounds.InProgressRound)
            {
                if (id == null)
                {
                    Log.Info($"{ConsoleSystem.Caller.Name} tried to change his/her role when the game hadn't started.");
                }
                else
                {
                    Log.Info($"{ConsoleSystem.Caller.Name} tried to change role of ID {id} when the game hadn't started.");
                }

                return;
            }

            Type type = Utils.GetTypeByLibraryName<TTTRole>(roleName);

            if (type == null)
            {
                Log.Info($"{ConsoleSystem.Caller.Name} entered a wrong role name: '{roleName}'.");

                return;
            }

            TTTRole role = Utils.GetObjectByType<TTTRole>(type);

            if (role == null)
            {
                return;
            }

            TTTPlayer player = null;

            if (id == null)
            {
                player = ConsoleSystem.Caller.Pawn as TTTPlayer;
            }
            else
            {
                try
                {
                    player = GetPlayerById(int.Parse(id));
                }
                catch (Exception)
                {
                    return;
                }
            }

            if (player == null)
            {
                return;
            }

            player.SetRole(role);
            player.SendClientRole();
        }

        [ServerCmd(Name = "ttt_forcespec")]
        public static void ToggleForceSpectator()
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid() || player.IsInitialSpawning)
            {
                return;
            }

            player.ToggleForcedSpectator();
        }

        [ServerCmd(Name = "ttt_force_restart")]
        public static void ForceRestart()
        {
            if (!ConsoleSystem.Caller.HasPermission("restart"))
            {
                return;
            }

            Gamemode.Game.Instance.ChangeRound(new PreRound());

            Log.Info($"{ConsoleSystem.Caller.Name} forced a restart.");
        }
    }
}
