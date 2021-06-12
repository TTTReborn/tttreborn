using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Hud : HudEntity<RootPanel>
    {
        public static Hud Instance { set; get; }

        private PlayerInfo playerInfo;
        private WeaponSelection weaponSelection;

        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            Instance = this;

            // s&box defaults
            RootPanel.AddChild<ChatBox>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<GameTimer>();
            RootPanel.AddChild<Scoreboard>();
            RootPanel.AddChild<WeaponSelection>();
            RootPanel.AddChild<InfoFeed>();
            RootPanel.AddChild<InspectMenu>();
            RootPanel.AddChild<PlayerInfo>();
        }

        [Event("tttreborn.player.died")]
        public void OnPlayerDied()
        {
            playerInfo?.Delete();
            playerInfo = null;

            weaponSelection?.Delete();
            weaponSelection = null;
        }

        [Event("tttreborn.player.spawned")]
        public void OnPlayerSpawned()
        {
            if (playerInfo != null || weaponSelection != null)
            {
                return;
            }

            playerInfo = RootPanel.AddChild<PlayerInfo>();
            weaponSelection = RootPanel.AddChild<WeaponSelection>();
        }
    }
}
