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
        public static Dictionary<int, TTTRoleButtonData> RoleButtons = new();
        public static Dictionary<int, RoleButtonPoint> RoleButtonPoints = new();
        public static RoleButtonPoint FocusedButton;
        public bool HasTrackedButtons => RoleButtons.Count > 0; // RoleButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

        public void SendRoleButtonsToClient()
        {
            if (IsClient)
            {
                return;
            }

            List<TTTRoleButtonData> roleButtonDataList = new();

            foreach (Entity entity in All)
            {
                if (entity is TTTRoleButton roleButton && roleButton.Role.Equals(Role.Name))
                {
                    roleButtonDataList.Add(roleButton.PackageData());
                }
            }

            // Network a small amount of data for each button within the player's scope.
            ClientStoreRoleButton(To.Single(this), roleButtonDataList.ToArray());
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
                player.SendRoleButtonsToClient();
            }
        }

        [Event(TTTEvent.UI.Reloaded)]
        public static void OnUIReloaded()
        {
            RoleButtonPoints = new();

            foreach (KeyValuePair<int, TTTRoleButtonData> keyValuePair in RoleButtons)
            {
                RoleButtonPoints.Add(keyValuePair.Key, new RoleButtonPoint(keyValuePair.Value));
            }
        }

        // Receive data of player's buttons from client.
        [ClientRpc]
        public void ClientStoreRoleButton(TTTRoleButtonData[] buttons)
        {
            Clear();

            FocusedButton = null;

            // Index our data table by the role buttons network identity so we can find it later if need be.
            RoleButtons = buttons.ToDictionary(k => k.NetworkIdent, v => v);
            RoleButtonPoints = buttons.ToDictionary(k => k.NetworkIdent, v => new RoleButtonPoint(v));
        }

        // Clear role buttons, called before player respawns.
        [ClientRpc]
        public void RemoveRoleButtons()
        {
            Clear();
        }

        private void Clear()
        {
            foreach (RoleButtonPoint roleButtonPoint in RoleButtonPoints.Values)
            {
                roleButtonPoint.Delete(true);
            }

            RoleButtons.Clear();
            RoleButtonPoints.Clear();
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

            IEnumerable<TTTRoleButton> roleButtons = All.Where(x => x is TTTRoleButton).Select(x => x as TTTRoleButton);
            IEnumerable<TTTRoleButton> applicableButtons = roleButtons.Where(x => x.Role.Equals(Utils.GetLibraryName(typeof(TraitorRole))));

            player.ClientStoreRoleButton(To.Single(player), applicableButtons.Select(x => x.PackageData()).ToArray());
        }

        // Handle client telling server to activate a specific button
        [ServerCmd]
        public static void ActivateRoleButton(int networkIdent)
        {
            if (ConsoleSystem.Caller.Pawn is not TTTPlayer player)
            {
                Log.Warning("Server received call from null player to activate role button.");

                return;
            }

            Entity entity = FindByIndex(networkIdent);

            if (entity == null || entity is not TTTRoleButton button)
            {
                Log.Warning($"Server received call for null role button with network id `{networkIdent}`.");

                return;
            }

            if (button.CanUse())
            {
                button.Press(player);
            }
        }

        // Client keybinding for activating button within focus.
        public void TickRoleButtonActivate()
        {
            if (!IsClient || Local.Pawn is not TTTPlayer player || FocusedButton == null || !Input.Pressed(InputButton.Use))
            {
                return;
            }

            // Double check all of our data that initially set `FocusedButton` to make sure nothing has changed or any fuckery is about.
            if (FocusedButton.IsLengthWithinCamerasFocus() && FocusedButton.IsUsable(player))
            {
                ActivateRoleButton(FocusedButton.Data.NetworkIdent);
            }
        }
    }
}
