using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Hud : HudEntity<RootPanel>
    {
        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            RootPanel.AddChild<ChatBox>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<GameTimer>();
            RootPanel.AddChild<InfoFeed>();
            RootPanel.AddChild<InspectMenu>();
            RootPanel.AddChild<PlayerInfo>();
            RootPanel.AddChild<WeaponSelection>();
        }

        [ClientRpc]
        public void OnPlayerDied(string victim, string attacker = null)
        {
            Host.AssertClient();
        }

        [ClientRpc]
        public void ShowDeathScreen(string attackerName)
        {
            Host.AssertClient();
        }
    }
}
