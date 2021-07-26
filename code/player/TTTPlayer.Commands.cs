using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        [ServerCmd(Name = "ttt_respawn", Help = "Respawns the current player")]
        public static void RespawnPlayer()
        {
            if (!ConsoleSystem.Caller.HasPermission("respawn"))
            {
                return;
            }

            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid() || ConsoleSystem.Caller.GetScore<bool>("forcedspectator", false))
            {
                Log.Info($"You tried to respawn yourself while you've been a forced spectator this round.");

                return;
            }

            player.Respawn();

            Log.Info($"You respawned yourself.");

            return;
        }

        [ServerCmd(Name = "ttt_respawnid", Help = "Respawns the player with the associated ID")]
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
                            if (playerList[i].GetScore<bool>("forcedspectator", false))
                            {
                                Log.Info($"You tried to spawn the player '{playerList[i].Name}' who have been a forced spectator this round.");

                                return;
                            }

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

        [ServerCmd(Name = "ttt_requestitem")]
        public static void RequestItem(string itemName)
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid())
            {
                return;
            }

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

            if (item == null)
            {
                return;
            }

            player.RequestPurchase(item);
        }

        [ServerCmd(Name = "ttt_setrole")]
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

            Type type = Utils.GetTypeByName<TTTRole>(roleName);

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

            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid())
            {
                return;
            }

            player.SetRole(role);
            RPCs.ClientSetRole(To.Single(player), player, role.Name);
        }

        [ClientCmd(Name = "ttt_playerids", Help = "Returns a list of all players (clients) and their associated IDs")]
        public static void PlayerID()
        {
            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.ToList().Count; i++)
            {
                Log.Info($"Player (ID: '{i}'): {playerList[i].Name}");
            }
        }

        [ServerCmd(Name = "ttt_forcespec")]
        public static void ToggleForceSpectator()
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid() || player.IsInitialSpawning)
            {
                return;
            }

            player.IsForcedSpectator = !player.IsForcedSpectator;

            if (player.IsForcedSpectator && player.LifeState == LifeState.Alive)
            {
                player.MakeSpectator(false);

                if (!ConsoleSystem.Caller.GetScore<bool>("forcedspectator", false))
                {
                    ConsoleSystem.Caller.SetScore("forcedspectator", true);
                }
            }

            string toggle = player.IsForcedSpectator ? "activated" : "deactivated";

            Log.Info($"You {toggle} force spectator.");
        }
    }
}
