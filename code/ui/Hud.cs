using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class Hud : HudEntity<RootPanel>
    {
        public static Hud Current { set; get; }

        public GeneralHud GeneralHudPanel;
        public AliveHud AliveHudPanel;

        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            Current = this;

            GeneralHudPanel = new GeneralHud(RootPanel);
            AliveHudPanel = new AliveHud(RootPanel);
        }

        [Event.Hotload]
        public static void OnHotReloaded()
        {
            if (Host.IsClient)
            {
                Local.Hud?.Delete();
                Hud hud = new Hud();
                if (Local.Client.Pawn is TTTPlayer player && player.LifeState == LifeState.Alive)
                {
                    hud.AliveHudPanel.CreateHud();
                }
            }
        }

        [Event("tttreborn.player.spawned")]
        private void OnPlayerSpawned(TTTPlayer player)
        {
            if (player != Local.Client.Pawn)
            {
                return;
            }

            Current?.AliveHudPanel.CreateHud();
        }

        [Event("tttreborn.player.died")]
        private void OnPlayerDied(TTTPlayer player)
        {
            if (player != Local.Client.Pawn)
            {
                return;
            }

            Current?.AliveHudPanel.DeleteHud();
        }

        public class GeneralHud : Panel
        {
            public Scoreboard Scoreboard;

            public GeneralHud(Panel parent)
            {
                Parent = parent;

                Parent.AddChild<ChatBox>();
                Parent.AddChild<VoiceList>();
                Parent.AddChild<GameTimer>();
                Parent.AddChild<InfoFeed>();
                Parent.AddChild<PostRoundMenu>();
                Scoreboard = Parent.AddChild<Scoreboard>();
            }
        }

        public class AliveHud : Panel
        {
            private PlayerInfo _playerInfo;
            private WeaponSelection _weaponSelection;
            private InspectMenu _inspectMenu;
            private Nameplate _nameplate;
            private QuickShop _quickShop;

            public AliveHud(Panel parent)
            {
                Parent = parent;
            }

            public void CreateHud()
            {
                _playerInfo ??= Parent.AddChild<PlayerInfo>();
                _weaponSelection ??= Parent.AddChild<WeaponSelection>();
                _inspectMenu ??= Parent.AddChild<InspectMenu>();
                _nameplate ??= Parent.AddChild<Nameplate>();
                _quickShop ??= Parent.AddChild<QuickShop>();
            }

            public void DeleteHud()
            {
                _playerInfo?.Delete();
                _playerInfo = null;
                _weaponSelection?.Delete();
                _weaponSelection = null;
                _inspectMenu?.Delete();
                _inspectMenu = null;
                _nameplate?.Delete();
                _nameplate = null;
                _quickShop?.Delete();
                _quickShop = null;
            }
        }
    }
}
