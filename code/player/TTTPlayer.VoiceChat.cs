using Sandbox;

using TTTReborn.Teams;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public bool IsTeamVoiceChatEnabled { get; private set; } = false;

        public void TickPlayerVoiceChat()
        {
            if (Input.Down(InputButton.Voice) || IsTeamVoiceChatEnabled)
            {
                UI.VoiceList.Current?.OnVoicePlayed(GetClientOwner().SteamId, 1f);
            }

            if (Input.Pressed(InputButton.Run) && CanUseTeamVoiceChat(this))
            {
                ConsoleSystem.Run("requestteamchat", true);
            }
            else if (Input.Released(InputButton.Run) && IsTeamVoiceChatEnabled)
            {
                ConsoleSystem.Run("requestteamchat", false);
            }
        }

        [ServerCmd(Name = "requestteamchat")]
        public static void RequestTeamChat(bool toogle)
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid() || toogle && !CanUseTeamVoiceChat(player))
            {
                return;
            }

            player.IsTeamVoiceChatEnabled = toogle;

            player.ClientToggleTeamVoiceChat(To.Single(player), toogle);
        }

        public static bool CanUseTeamVoiceChat(TTTPlayer player)
        {
            return player.Team == TTTTeam.GetTeam("Traitors");
        }

        [ClientRpc]
        public void ClientToggleTeamVoiceChat(bool toggle)
        {
            IsTeamVoiceChatEnabled = toggle;

            // De-/Activate voice chat
            ConsoleSystem.Run(toggle ? "+" : "-" + "iv_voice");
        }
    }
}
