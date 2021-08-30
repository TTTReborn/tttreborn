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
            }
        }

        public class GeneralHud : TTTPanel
        {
            public GeneralHud(Panel parent)
            {
                Parent = parent;

                Parent.AddChild<PlayerInfo>();
                Parent.AddChild<InventoryWrapper>();
                Parent.AddChild<ChatBox>();
                Parent.AddChild<VoiceList>();
                Parent.AddChild<Nameplate>();
                Parent.AddChild<GameTimer>();
                Parent.AddChild<InfoFeed>();
                Parent.AddChild<InspectMenu>();
                Parent.AddChild<PostRoundMenu>();
                Parent.AddChild<Scoreboard>();
                Parent.AddChild<Menu.Menu>();
            }
        }

        public class AliveHud : TTTPanel
        {
            public AliveHud(Panel parent)
            {
                Parent = parent;

                Parent.AddChild<DrowningIndicator>();
                Parent.AddChild<QuickShop>();
                Parent.AddChild<DamageIndicator>();
            }
        }
    }
}
