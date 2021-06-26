using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [ServerCmd(Name = "respawn", Help = "Respawns the current player")]
        public static void RespawnPlayer()
        {
            if (!ConsoleSystem.Caller.HasPermission("respawn"))
            {
                return;
            }

            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid())
            {
                return;
            }

            player.Respawn();

            Log.Info($"You respawned yourself.");

            return;
        }

        [ServerCmd(Name = "respawnid", Help = "Respawns the player with the associated ID")]
        public static void RespawnPlayer(int id)
        {
            if (!ConsoleSystem.Caller.HasPermission("respawn"))
            {
                return;
            }

            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.ToList().Count; i++)
            {
                if (i == id)
                {
                    if (playerList[i].Pawn is TTTPlayer player)
                    {
                        if (player.IsValid())
                        {
                            player.Respawn();

                            Log.Info($"You've respawned the client '{playerList[i].Name}'.");
                        }
                        else
                        {
                            Log.Info($"'{playerList[i].Name}' does not have a valid client's pawn.");
                        }
                    }

                    return;
                }
            }
        }

        [ServerCmd(Name = "requestitem")]
        public static void RequestItem(string itemName)
        {
            IBuyableItem item = null;

            Library.GetAll<IBuyableItem>().ToList().ForEach(t =>
            {
                if (!t.IsAbstract && !t.ContainsGenericParameters)
                {
                    if (Library.GetAttribute(t).Name == itemName)
                    {
                        item = Library.Create<IBuyableItem>(t);
                    }
                }
            });

            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (item == null || !player.IsValid())
            {
                return;
            }

            player.RequestPurchase(item);
        }

        [ServerCmd(Name = "setrole")]
        public static void SetRole(string roleName)
        {
            if (!ConsoleSystem.Caller.HasPermission("role"))
            {
                return;
            }

            if (Gamemode.Game.Instance.Round is not Rounds.InProgressRound)
            {
                Log.Info($"{ConsoleSystem.Caller.Name} tried to change his/her role when the game hadn't started.");

                return;
            }

            Type type = RoleFunctions.GetRoleTypeByName(roleName);

            if (type == null)
            {
                Log.Info($"{ConsoleSystem.Caller.Name} entered a wrong role name: '{roleName}'.");

                return;
            }

            TTTRole role = RoleFunctions.GetRoleByType(type);

            if (role == null)
            {
                return;
            }

            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid())
            {
                return;
            }

            player.SetRole(role);
            player.ClientSetRole(To.Single(player), role.Name);
        }

        [ClientCmd(Name = "playerids", Help = "Returns a list of all players (clients) and their associated IDs")]
        public static void PlayerID()
        {
            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.ToList().Count; i++)
            {
                Log.Info($"Player (ID: '{i}'): {playerList[i].Name}");
            }
        }
    }
}
