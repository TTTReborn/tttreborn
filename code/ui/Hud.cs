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
        private InventorySelection _inventorySelection;
        private InspectMenu _inspectMenu;
        private Nameplate _nameplate;

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
            RootPanel.AddChild<InfoFeed>();
            RootPanel.AddChild<PostRoundMenu>();
            RootPanel.AddChild<QuickShop>();

            Scoreboard = RootPanel.AddChild<Scoreboard>();
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

            _inventorySelection?.Delete();
            _inventorySelection = null;

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

            _inventorySelection ??= RootPanel.AddChild<InventorySelection>();

            _inspectMenu ??= RootPanel.AddChild<InspectMenu>();

            _nameplate ??= RootPanel.AddChild<Nameplate>();
        }
    }
}
