using Sandbox;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public void TickPlayerVoiceChat()
        {
            if (Input.Down(InputButton.Voice))
            {
                UI.VoiceList.Current?.OnVoicePlayed(GetClientOwner().SteamId, 1f);
            }
        }
    }
}
