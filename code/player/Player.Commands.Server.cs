using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Items;
using TTTReborn.Roles;
using TTTReborn.Rounds;

namespace TTTReborn
{
    public partial class Player
    {
        private static Player GetPlayerById(int id)
        {
            List<Client> playerList = Client.All.ToList();

            if (playerList.Count <= id)
            {
                return null;
            }

            if (playerList[id].Pawn is Player player && player.IsValid())
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

            Player player;

            if (id == null)
            {
                player = ConsoleSystem.Caller.Pawn as Player;
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

            Player player = ConsoleSystem.Caller.Pawn as Player;

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

            Type type = Utils.GetTypeByLibraryName<Role>(roleName);

            if (type == null)
            {
                Log.Info($"{ConsoleSystem.Caller.Name} entered a wrong role name: '{roleName}'.");

                return;
            }

            Role role = Utils.GetObjectByType<Role>(type);

            if (role == null)
            {
                return;
            }

            Player player;

            if (id == null)
            {
                player = ConsoleSystem.Caller.Pawn as Player;
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
            Player player = ConsoleSystem.Caller.Pawn as Player;

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
