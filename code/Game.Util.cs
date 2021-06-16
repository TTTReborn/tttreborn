using System.Collections.Generic;
using Sandbox;
using TTTReborn.Player;

namespace TTTReborn.Gamemode
{
    public partial class Game
    {
        public static List<TTTPlayer> GetPlayers()
        {
            List<TTTPlayer> players = new List<TTTPlayer>();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer player)
                {
                    players.Add(player);
                }
            }

            return players;
        }

        public static List<TTTPlayer> GetAlivePlayers()
        {
            List<TTTPlayer> players = new List<TTTPlayer>();

            foreach (Client client in Client.All)
            {
                if (client.Pawn is TTTPlayer player && player.LifeState == LifeState.Alive)
                {
                    players.Add(player);
                }
            }

            return players;
        }
    }
}
