using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [ServerCmd(Name = "respawn", Help = "Respawns the current player")]
        public static void RespawnPlayer()
        {
            (ConsoleSystem.Caller.Pawn as TTTPlayer).Respawn();

            Log.Info($"You respawned yourself.");

            return;
        }

        [ServerCmd(Name = "respawnid", Help = "Respawns the player with the associated ID")]
        public static void RespawnPlayer(int id)
        {
            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.ToList().Count; i++)
            {
                if (i == id)
                {
                    if (playerList[i].Pawn is TTTPlayer player)
                    {
                        player.Respawn();

                        Log.Info($"You've respawned the client '{playerList[i].Name}'.");
                    }

                    return;
                }
            }
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
