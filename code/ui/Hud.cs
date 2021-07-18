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
        public DeadHud DeadHudPanel;

        public Hud()
        {
            if (!IsClient)
            {
                return;
            }

            Current = this;

            GeneralHudPanel = new GeneralHud(RootPanel);
            AliveHudPanel = new AliveHud(RootPanel);
            DeadHudPanel = new DeadHud(RootPanel);
        }

        [Event.Hotload]
        public static void OnHotReloaded()
        {
            if (Host.IsClient)
            {
                Local.Hud?.Delete();

                Hud hud = new Hud();

                if (Local.Client.Pawn is TTTPlayer player)
                {
                    if (player.LifeState == LifeState.Alive)
                    {
                        hud.AliveHudPanel.CreateHud();
                    }
                    else
                    {
                        hud.DeadHudPanel.CreateHud();
                    }
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

            Current?.DeadHudPanel.DeleteHud();
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
            Current?.DeadHudPanel.CreateHud();
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
            public DamageIndicator DamageIndicator;
            public InventoryWrapper InventoryWrapper;
            public PlayerInfo PlayerInfo;
            public InspectMenu InspectMenu;
            public Nameplate Nameplate;
            public QuickShop QuickShop;
            public DrowningIndicator DrowningIndicator;

            public AliveHud(Panel parent)
            {
                Parent = parent;
            }

            public void CreateHud()
            {
                InventoryWrapper ??= Parent.AddChild<InventoryWrapper>();
                DamageIndicator ??= Parent.AddChild<DamageIndicator>();
                PlayerInfo ??= Parent.AddChild<PlayerInfo>();
                InspectMenu ??= Parent.AddChild<InspectMenu>();
                Nameplate ??= Parent.AddChild<Nameplate>();
                QuickShop ??= Parent.AddChild<QuickShop>();
                DrowningIndicator ??= Parent.AddChild<DrowningIndicator>();
            }

            public void DeleteHud()
            {
                DamageIndicator?.Delete();
                DamageIndicator = null;

                InventoryWrapper?.Delete();
                InventoryWrapper = null;

                PlayerInfo?.Delete();
                PlayerInfo = null;

                InspectMenu?.Delete();
                InspectMenu = null;

                Nameplate?.Delete();
                Nameplate = null;

                QuickShop?.Delete();
                QuickShop = null;

                DrowningIndicator?.Delete();
                DrowningIndicator = null;
            }
        }

        public class DeadHud : Panel
        {
            public SpectatedPlayerInfo SpectatedPlayerInfo;

            public DeadHud(Panel parent)
            {
                Parent = parent;
            }

            public void CreateHud()
            {
                SpectatedPlayerInfo ??= Parent.AddChild<SpectatedPlayerInfo>();
            }

            public void DeleteHud()
            {
                SpectatedPlayerInfo?.Delete();
                SpectatedPlayerInfo = null;
            }
        }
    }
}
