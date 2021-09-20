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

            GeneralHudPanel = RootPanel.AddChild<GeneralHud>();
            AliveHudPanel = RootPanel.AddChild<AliveHud>();
            Current = this;
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
                    hud.AliveHudPanel.Enabled = true;
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

            AliveHudPanel.Enabled = true;
        }

        [Event("tttreborn.player.died")]
        private void OnPlayerDied(TTTPlayer deadPlayer)
        {
            if (deadPlayer != Local.Client.Pawn)
            {
                return;
            }

            AliveHudPanel.Enabled = false;
        }

        public class GeneralHud : Panel
        {
            public GeneralHud()
            {
                AddClass("fullscreen");
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
                AddChild<Scoreboard>();
                AddChild<Menu.Menu>();
            }
        }

        public class AliveHud : Panel
        {
            public AliveHud()
            {
                AddClass("fullscreen");

                AddChild<BreathIndicator>();
                AddChild<StaminaIndicator>();
                AddChild<QuickShop>();
                AddChild<DamageIndicator>();
            }
        }
    }
}
