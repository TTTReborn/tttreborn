using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Events;
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
        public static bool HasTrackedButtons => LogicButtons.Count > 0; // LogicButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

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

        [Event(TTTEvent.UI.RELOADED)]
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
