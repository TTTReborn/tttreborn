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

                Hud hud = new();

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
            public GeneralHud(Sandbox.UI.Panel parent)
            {
                Parent = parent;

                Parent.AddChild<Crosshair>();
                Parent.AddChild<PlayerInfo>();
                Parent.AddChild<InventoryWrapper>();
                Parent.AddChild<ChatBox>();
                Parent.AddChild<VoiceChatDisplay>();
                Parent.AddChild<Nameplate>();
                Parent.AddChild<GameTimerDisplay>();
                Parent.AddChild<InfoFeed>();
                Parent.AddChild<InspectMenu>();
                Parent.AddChild<PostRoundMenu>();
                Parent.AddChild<Scoreboard>();
                Parent.AddChild<Menu.Menu>();
            }
        }

        public class AliveHud : Panel
        {
            public AliveHud(Sandbox.UI.Panel parent)
            {
                Parent = parent;
            }

            public void CreateHud()
            {
                Parent.AddChild<DamageIndicator>();
                Parent.AddChild<QuickShop>();
                Parent.AddChild<DrowningIndicator>();
            }

            public void DeleteHud()
            {
                DeleteChildren(true);
            }
        }
    }
}
