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
        private InspectMenu _inspectMenu;
        private Nameplate _nameplate;

        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            Instance = this;

            RootPanel.AddChild<Chat>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<GameTimer>();
            RootPanel.AddChild<Scoreboard>();
            RootPanel.AddChild<InfoFeed>();
            RootPanel.AddChild<PostRoundMenu>();
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

            _inspectMenu?.Delete();
            _inspectMenu = null;

            _nameplate?.Delete();
            _nameplate = null;
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

            _inspectMenu ??= RootPanel.AddChild<InspectMenu>();

            _nameplate ??= RootPanel.AddChild<Nameplate>();
        }
    }
}
