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

            Current.AliveHudPanel.Enabled = true;
        }

        [Event("tttreborn.player.died")]
        private void OnPlayerDied(TTTPlayer player)
        {
            if (player != Local.Client.Pawn)
            {
                return;
            }

            Current.AliveHudPanel.Enabled = false;
        }

        public class GeneralHud : Panel
        {
            public GeneralHud(Sandbox.UI.Panel parent) : base(parent)
            {
                Parent = parent;

                Parent.AddChild<RadarDisplay>();
                Parent.AddChild<Crosshair>();
                Parent.AddChild<PlayerRoleDisplay>();
                Parent.AddChild<PlayerInfoDisplay>();
                Parent.AddChild<InventoryWrapper>();
                Parent.AddChild<ChatBox>();

                Parent.AddChild<VoiceChatDisplay>();
                Parent.AddChild<Nameplate>();
                Parent.AddChild<GameTimerDisplay>();

                Parent.AddChild<VoiceList>();

                Parent.AddChild<InfoFeed>();
                Parent.AddChild<InspectMenu>();
                Parent.AddChild<PostRoundMenu>();
                Parent.AddChild<Scoreboard>();
                Parent.AddChild<Menu.Menu>();
            }
        }

        public class AliveHud : Panel
        {
            public AliveHud(Sandbox.UI.Panel parent) : base(parent)
            {
                Parent = parent;

                Parent.AddChild<DrowningIndicator>();
                Parent.AddChild<QuickShop>();
                Parent.AddChild<DamageIndicator>();
            }
        }
    }
}
