using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Hud : HudEntity<RootPanel>
    {
        public static Hud Instance { set; get; }

        private PlayerInfo _playerInfo;
        private WeaponSelection _weaponSelection;

        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            Instance = this;

            RootPanel.AddChild<ChatBox>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<GameTimer>();
            RootPanel.AddChild<Scoreboard>();
            RootPanel.AddChild<InfoFeed>();
            RootPanel.AddChild<PostRoundMenu>();
            RootPanel.AddChild<InspectMenu>();
            RootPanel.AddChild<Nameplate>();
            RootPanel.AddChild<QuickShop>();
        }

        [Event("tttreborn.player.died")]
        private void OnPlayerDied(TTTPlayer player)
        {
            if (player != Local.Client.Pawn)
            {
                return;
            }

            _playerInfo?.Delete();
            _playerInfo = null;

            _weaponSelection?.Delete();
            _weaponSelection = null;
        }

        [Event("tttreborn.player.spawned")]
        private void OnPlayerSpawned(TTTPlayer player)
        {
            if (player != Local.Client.Pawn)
            {
                return;
            }

            _playerInfo ??= RootPanel.AddChild<PlayerInfo>();

            _weaponSelection ??= RootPanel.AddChild<WeaponSelection>();
        }
    }
}
