using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace TTTReborn
{
    public partial class Player
    {
        [ConCmd.Client(Name = "ttt_playerids", Help = "Returns a list of all players (clients) and their associated IDs")]
        public static void PlayerID()
        {
            List<IClient> playerList = Game.Clients.ToList();

            for (int i = 0; i < playerList.Count; i++)
            {
                Log.Info($"Player (ID: '{i}'): {playerList[i].Name}");
            }
        }
    }
}
