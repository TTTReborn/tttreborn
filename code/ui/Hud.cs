using System.Collections.Generic;

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
            public DamageIndicator DamageIndicator;
            public Effects Effects;
            public PlayerInfo PlayerInfo;
            public InventorySelection InventorySelection;
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
                DamageIndicator ??= Parent.AddChild<DamageIndicator>();
                Effects ??= Parent.AddChild<Effects>();
                PlayerInfo ??= Parent.AddChild<PlayerInfo>();
                InventorySelection ??= Parent.AddChild<InventorySelection>();
                InspectMenu ??= Parent.AddChild<InspectMenu>();
                Nameplate ??= Parent.AddChild<Nameplate>();
                QuickShop ??= Parent.AddChild<QuickShop>();
                DrowningIndicator ??= Parent.AddChild<DrowningIndicator>();
            }

            public void DeleteHud()
            {
                DamageIndicator?.Delete();
                DamageIndicator = null;

                Effects?.Delete();
                Effects = null;

                PlayerInfo?.Delete();
                PlayerInfo = null;

                InventorySelection?.Delete();
                InventorySelection = null;

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
    }
}
