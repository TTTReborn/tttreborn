using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Map;
using TTTReborn.Roles;
using TTTReborn.UI;

#pragma warning disable CA1822

namespace TTTReborn
{
    public partial class Player
    {
        public static Dictionary<int, LogicButtonData> LogicButtons { get; set; } = new();
        public static Dictionary<int, LogicButtonPoint> LogicButtonPoints { get; set; } = new();
        public static LogicButtonPoint FocusedButton { get; set; }
        public static bool HasTrackedButtons => LogicButtons.Count > 0; // LogicButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

        public void SendLogicButtonsToClient()
        {
            if (IsClient)
            {
                return;
            }

            List<LogicButtonData> logicButtonDataList = new();

            foreach (Entity entity in All)
            {
                if (entity is LogicButton logicButton && (logicButton.CheckValue.Equals(Role.Name) || logicButton.CheckValue.Equals(Team.Name)))
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

            foreach (Player player in Utils.GetPlayers())
            {
                player.SendLogicButtonsToClient();
            }
        }

        [Event("ui_reloaded")]
        public static void OnUIReloaded()
        {
            LogicButtonPoints = new();

            foreach (KeyValuePair<int, LogicButtonData> keyValuePair in LogicButtons)
            {
                LogicButtonPoints.Add(keyValuePair.Key, new LogicButtonPoint(keyValuePair.Value));
            }
        }

        // Receive data of player's buttons from client.
        [ClientRpc]
        public void ClientStoreLogicButton(LogicButtonData[] buttons)
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

        private static void Clear()
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
        [ConCmd.Server("ttt_debug_sendrb")]
        public static void ForceRBSend()
        {
            Player player = ConsoleSystem.Caller.Pawn as Player;

            if (!player.IsValid())
            {
                return;
            }

            IEnumerable<LogicButton> logicButtons = All.Where(x => x is LogicButton).Select(x => x as LogicButton);
            IEnumerable<LogicButton> applicableButtons = logicButtons.Where(x => x.CheckValue.Equals(Teams.TeamFunctions.GetTeam(typeof(Teams.TraitorTeam))) || x.CheckValue.Equals(Utils.GetLibraryName(typeof(TraitorRole))));

            player.ClientStoreLogicButton(To.Single(player), applicableButtons.Select(x => x.PackageData()).ToArray());
        }

        // Handle client telling server to activate a specific button
        [ConCmd.Server]
        public static void ActivateLogicButton(int networkIdent)
        {
            if (ConsoleSystem.Caller.Pawn is not Player player)
            {
                Log.Warning("Server received call from null player to activate logic button.");

                return;
            }

            Entity entity = FindByIndex(networkIdent);

            if (entity == null || entity is not LogicButton button)
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
        public static void TickLogicButtonActivate()
        {
            if (Local.Pawn is not Player player || FocusedButton == null || !Input.Pressed(InputButton.Use))
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
