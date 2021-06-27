using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Hud : HudEntity<RootPanel>
    {
        public static Hud Instance { set; get; }

        public Scoreboard Scoreboard;

        private PlayerInfo _playerInfo;
        private WeaponSelection _weaponSelection;
        private InspectMenu _inspectMenu;
        private Nameplate _nameplate;

        public Hud() => BuildHud();

        [Event("hotloaded")]
        private void BuildHud()
        {
            if (!IsClient)
            {
                return;
            }

            Instance = this;

            RootPanel.DeleteChildren(true);

            RootPanel.AddChild<ChatBox>();
            RootPanel.AddChild<VoiceList>();
            RootPanel.AddChild<GameTimer>();
            RootPanel.AddChild<InfoFeed>();
            RootPanel.AddChild<PostRoundMenu>();
            RootPanel.AddChild<QuickShop>();

            Scoreboard = RootPanel.AddChild<Scoreboard>();

            if (!Local.Client.IsValid() || !Local.Client.Pawn.IsValid())
            {
                return;
            }

            TTTPlayer player = Local.Client.Pawn as TTTPlayer;

            OnPlayerDied(player);

            if (player.LifeState == LifeState.Alive)
            {
                OnPlayerSpawned(player as TTTPlayer);
            }
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
