using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Hud : HudEntity<RootPanel>
    {
        public static Hud Current { set; get; }

        public GeneralHud GeneralHudPanel;
        public AliveHud AliveHudInstance;

        public Hud()
        {
            if (Host.IsServer)
            {
                return;
            }

            RootPanel.StyleSheet.Load("/ui/Hud.scss");
            RootPanel.AddClass("panel");

            GeneralHudPanel = RootPanel.AddChild<GeneralHud>();
            AliveHudInstance = new(RootPanel);
            Current = this;
        }

        [Event.Hotload]
        public static void OnHotReloaded()
        {
            if (Host.IsServer)
            {
                return;
            }

            Current?.Delete();

            _ = new Hud();

            if (Local.Client.Pawn is Player player)
            {
                Current.AliveHudInstance.Enabled = player.LifeState == LifeState.Alive && !player.IsForcedSpectator;
            }

            GameEvent.Register(new Events.UI.ReloadedEvent());
        }

        [Event("player_spawn")]
        protected void OnPlayerSpawned(Player player)
        {
            if (IsServer || player != Local.Client.Pawn)
            {
                return;
            }

            AliveHudInstance.Enabled = !player.IsSpectator && !player.IsForcedSpectator;
        }

        [Event("player_died")]
        protected void OnPlayerDied(Player deadPlayer)
        {
            if (IsServer || deadPlayer != Local.Client.Pawn)
            {
                return;
            }

            AliveHudInstance.Enabled = false;
        }

        [Event("player_initialspawn")]
        public static void Initialize(Client client)
        {
            if (Host.IsServer || client != Local.Client)
            {
                return;
            }

            _ = new Hud();
        }

        public class GeneralHud : Panel
        {
            public GeneralHud()
            {
                AddClass("fullscreen");
                AddChild<WIPDisclaimer>();

                AddChild<HintDisplay>();
                AddChild<RadarDisplay>();
                AddChild<PlayerRoleDisplay>();
                AddChild<PlayerInfoDisplay>();
                AddChild<InventoryWrapper>();
                AddChild<ChatBox>();

                AddChild<VoiceChatDisplay>();
                AddChild<GameTimerDisplay>();

                AddChild<VoiceList>();

                AddChild<InfoFeed>();
                AddChild<InspectMenu>();
                AddChild<PostRoundMenu>();
                AddChild<MapSelectionMenu>();
                AddChild<TabMenu>();
            }

            // Use "GeneralHud" as the Panel that displays any s&box popups.
            public override Panel FindPopupPanel()
            {
                return this;
            }
        }

        public class AliveHud
        {
            public bool Enabled
            {
                get => _enabled;
                internal set
                {
                    _enabled = value;

                    if (value)
                    {
                        Create();
                    }
                    else
                    {
                        Destroy();
                    }
                }
            }
            private bool _enabled = false;

            private readonly RootPanel _rootPanel;

            private List<Panel> _panelList = new();

            public AliveHud(RootPanel rootPanel)
            {
                _rootPanel = rootPanel;
            }

            private void Create()
            {
                _panelList = new()
                {
                    _rootPanel.AddChild<Crosshair>(),
                    _rootPanel.AddChild<BreathIndicator>(),
                    _rootPanel.AddChild<StaminaIndicator>(),
                    _rootPanel.AddChild<QuickShop>(),
                    _rootPanel.AddChild<DamageIndicator>(),
                    _rootPanel.AddChild<C4Arm>()
                };
            }

            private void Destroy()
            {
                _panelList.ForEach((panel) => panel.Delete(true));
                _panelList.Clear();
            }
        }
    }
}
