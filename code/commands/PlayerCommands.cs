using System.Collections.Generic;
using System.Linq;
using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Commands
{
    public static class PlayerCommands
    {
        [ServerCmd("Respawn")]
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
                    }

                    return;
                }
            }
        }

        [ClientCmd("PlayerID")]
        public static void PlayerID()
        {
            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.ToList().Count; i++)
            {
                if (playerList[i] == null)
                    continue;
                
                Log.Info("Player (ID: '" + i + "'): " + playerList[i].Name);
            }
        }
    }
}
