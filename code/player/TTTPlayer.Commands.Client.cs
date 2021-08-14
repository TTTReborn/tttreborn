using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.UI.Menu;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        [ClientCmd(Name = "ttt_playerids", Help = "Returns a list of all players (clients) and their associated IDs")]
        public static void PlayerID()
        {
            List<Client> playerList = Client.All.ToList();

            for (int i = 0; i < playerList.Count; i++)
            {
                Log.Info($"Player (ID: '{i}'): {playerList[i].Name}");
            }
        }

        [ClientCmd(Name = "ttt_menu")]
        public static void ToggleMenu()
        {
            Menu.Instance.IsShowing = !Menu.Instance.IsShowing;
        }

        [ClientCmd("ttt_language")]
        public static void ChangeLanguage(string name = null)
        {
            if (name is null)
            {
                Log.Info($"Your current language is set to '{TTTLanguage.ActiveLanguage.Data.Name}' ('{TTTLanguage.ActiveLanguage.Data.Code}').");

                return;
            }

            Language language = TTTLanguage.GetLanguageByCode(name);

            if (language is null)
            {
                Log.Warning($"Language '{name}' does not exist. Please enter an ISO (tag) code (http://www.lingoes.net/en/translator/langcode.htm).");

                return;
            }

            Log.Warning($"You set your language to '{language.Data.Name}'.");

            Settings.ClientSettings.Instance.Language = language.Data.Code;
        }
    }
}
