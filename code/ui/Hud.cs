// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Events;
using TTTReborn.Player;

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

            Hud.Current?.Delete();

            Hud hud = new();

            if (Local.Client.Pawn is TTTPlayer player)
            {
                Current.AliveHudInstance.Enabled = player.LifeState == LifeState.Alive && !player.IsForcedSpectator;
            }

            Event.Run(TTTEvent.UI.Reloaded);
        }

        [Event(TTTEvent.Player.Spawned)]
        private void OnPlayerSpawned(TTTPlayer player)
        {
            if (IsServer || player != Local.Client.Pawn)
            {
                return;
            }

            AliveHudInstance.Enabled = !player.IsSpectator && !player.IsForcedSpectator;
        }

        [Event(TTTEvent.Player.Died)]
        private void OnPlayerDied(TTTPlayer deadPlayer)
        {
            if (deadPlayer != Local.Client.Pawn)
            {
                return;
            }

            AliveHudInstance.Enabled = false;
        }

        [Event(TTTEvent.Player.InitialSpawn)]
        public static void Initialize(Client client)
        {
            if (Host.IsServer || client != Local.Client)
            {
                return;
            }

            Hud hud = new();
            hud.OnPlayerSpawned(client.Pawn as TTTPlayer); // InitialSpawn event is called after Spawned event, so we have to initialize manually
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
                AddChild<Scoreboard>();
                AddChild<MapSelectionMenu>();
                AddChild<Menu.Menu>();
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

            private RootPanel _rootPanel;

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
