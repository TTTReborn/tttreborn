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
using System.Linq;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Map;
using TTTReborn.Roles;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public static Dictionary<int, TTTLogicButtonData> LogicButtons = new();
        public static Dictionary<int, LogicButtonPoint> LogicButtonPoints = new();
        public static LogicButtonPoint FocusedButton;
        public bool HasTrackedButtons => LogicButtons.Count > 0; // LogicButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

        public void SendLogicButtonsToClient()
        {
            if (IsClient)
            {
                return;
            }

            List<TTTLogicButtonData> logicButtonDataList = new();

            foreach (Entity entity in All)
            {
                if (entity is TTTLogicButton logicButton && (logicButton.CheckValue.Equals(Role.Name) || logicButton.CheckValue.Equals(Team.Name)))
                {
                    logicButtonDataList.Add(logicButton.PackageData());
                }
            }

            // Network a small amount of data for each button within the player's scope.
            ClientStoreLogicButton(To.Single(this), logicButtonDataList.ToArray());
        }

        [Event.Hotload]
        public static void OnHotload()
        {
            if (Host.IsClient)
            {
                return;
            }

            foreach (TTTPlayer player in Utils.GetPlayers())
            {
                player.SendLogicButtonsToClient();
            }
        }

        [Event(TTTEvent.UI.Reloaded)]
        public static void OnUIReloaded()
        {
            LogicButtonPoints = new();

            foreach (KeyValuePair<int, TTTLogicButtonData> keyValuePair in LogicButtons)
            {
                LogicButtonPoints.Add(keyValuePair.Key, new LogicButtonPoint(keyValuePair.Value));
            }
        }

        // Receive data of player's buttons from client.
        [ClientRpc]
        public void ClientStoreLogicButton(TTTLogicButtonData[] buttons)
        {
            Clear();

            FocusedButton = null;

            // Index our data table by the Logic buttons network identity so we can find it later if need be.
            LogicButtons = buttons.ToDictionary(k => k.NetworkIdent, v => v);
            LogicButtonPoints = buttons.ToDictionary(k => k.NetworkIdent, v => new LogicButtonPoint(v));
        }

        // Clear logic buttons, called before player respawns.
        [ClientRpc]
        public void RemoveLogicButtons()
        {
            Clear();
        }

        private void Clear()
        {
            foreach (LogicButtonPoint logicButtonPoint in LogicButtonPoints.Values)
            {
                logicButtonPoint.Delete(true);
            }

            LogicButtons.Clear();
            LogicButtonPoints.Clear();
            FocusedButton = null;
        }

        // Debug method
        [ServerCmd("ttt_debug_sendrb")]
        public static void ForceRBSend()
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;

            if (!player.IsValid())
            {
                return;
            }

            IEnumerable<TTTLogicButton> logicButtons = All.Where(x => x is TTTLogicButton).Select(x => x as TTTLogicButton);
            IEnumerable<TTTLogicButton> applicableButtons = logicButtons.Where(x => x.CheckValue.Equals(Teams.TeamFunctions.GetTeam(typeof(Teams.TraitorTeam))) || x.CheckValue.Equals(Utils.GetLibraryName(typeof(TraitorRole))));

            player.ClientStoreLogicButton(To.Single(player), applicableButtons.Select(x => x.PackageData()).ToArray());
        }

        // Handle client telling server to activate a specific button
        [ServerCmd]
        public static void ActivateLogicButton(int networkIdent)
        {
            if (ConsoleSystem.Caller.Pawn is not TTTPlayer player)
            {
                Log.Warning("Server received call from null player to activate logic button.");

                return;
            }

            Entity entity = FindByIndex(networkIdent);

            if (entity == null || entity is not TTTLogicButton button)
            {
                Log.Warning($"Server received call for null logic button with network id `{networkIdent}`.");

                return;
            }

            if (button.CanUse())
            {
                button.Press(player);
            }
        }

        // Client keybinding for activating button within focus.
        public void TickLogicButtonActivate()
        {
            if (!IsClient || Local.Pawn is not TTTPlayer player || FocusedButton == null || !Input.Pressed(InputButton.Use))
            {
                return;
            }

            // Double check all of our data that initially set `FocusedButton` to make sure nothing has changed or any fuckery is about.
            if (FocusedButton.IsLengthWithinCamerasFocus() && FocusedButton.IsUsable(player))
            {
                ActivateLogicButton(FocusedButton.Data.NetworkIdent);
            }
        }
    }
}
