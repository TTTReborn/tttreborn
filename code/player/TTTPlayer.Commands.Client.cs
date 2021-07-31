using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        [ClientCmd(Name = "ttt_playerids", Help = "Returns a list of all players (clients) and their associated IDs")]
        public static void PlayerID()
        {
            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.ToList().Count; i++)
            {
                Log.Info($"Player (ID: '{i}'): {playerList[i].Name}");
            }
        }

        [ClientCmd(Name = "ttt_menu")]
        public static void ToggleMenu()
        {
            UI.Menu.Instance.IsShowing = !UI.Menu.Instance.IsShowing;
        }
    }
}
