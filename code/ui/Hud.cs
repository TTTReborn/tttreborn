using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    using System.Collections.Generic;
    using System.Linq;

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
            private List<Panel> _panels;

            public AliveHud(Panel parent)
            {
                Parent = parent;
                _panels = new List<Panel>();
            }

            public void CreateHud()
            {
                if (_panels.Count == 0)
                {
                    _panels = new List<Panel>()
                    {
                        Parent.AddChild<PlayerInfo>(),
                        Parent.AddChild<WeaponSelection>(),
                        Parent.AddChild<InspectMenu>(),
                        Parent.AddChild<Nameplate>(),
                        Parent.AddChild<QuickShop>()
                    };
                }
            }

            public void DeleteHud()
            {
                foreach (Panel child in _panels)
                {
                    child.Delete();
                }
                _panels.Clear();
            }
        }
    }
}
