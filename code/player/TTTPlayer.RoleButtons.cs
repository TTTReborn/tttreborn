using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Map;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        //Client
        public static Dictionary<int, TTTRoleButtonData> RoleButtons = new();
        public static Dictionary<int, RoleButtonPoint> RoleButtonPoints = new();
        public static RoleButtonPoint FocusedButton;
        public bool HasTrackedButtons => RoleButtons.Count > 0; //RoleButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

        //TODO: Call whenever player's role is set serverside, not initially during assignment.
        public void SendRoleButtonsToClient()
        {
            if (!IsServer)
            {
                return;
            }

            //Find all role button entities in the world and cast to TTTRoleButton.
            IEnumerable<TTTRoleButton> roleButtons = All.Where(x => x.GetType() == typeof(TTTRoleButton)).Select(x => x as TTTRoleButton);

            //Find specific role buttons to current player's role.
            IEnumerable<TTTRoleButton> applicableButtons = roleButtons.Where(x => x.Role.ToLower() == Role.Name.ToLower());

            //Network a small amount of data for each button within the player's scope.
            ClientStoreRoleButton(To.Single(Owner), applicableButtons.Select(x => x.PackageData()).ToArray());
        }

        //Receive data of player's buttons from client.
        [ClientRpc]
        public void ClientStoreRoleButton(TTTRoleButtonData[] buttons)
        {
            FocusedButton = null;

            //Index our data table by the role buttons network identity so we can find it later if need be.
            RoleButtons = buttons.ToDictionary(k => k.NetworkIdent, v => v);
            RoleButtonPoints = buttons.ToDictionary(k => k.NetworkIdent, v => new RoleButtonPoint(v));
        }

        //Clear role buttons, called before player respawns.
        [ClientRpc]
        public void RemoveRoleButtons()
        {
            RoleButtons = new();
            RoleButtonPoints = new();
            FocusedButton = null;
        }

        //Debug method
        /*
        [ServerCmd("ttt_sendrb")]
        public static void ForceRBSend() 
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;
        
            if (!player.IsValid())
            {
                return;
            }
        
            IEnumerable<TTTRoleButton> roleButtons = All.Where(x => x.GetType() == typeof(TTTRoleButton)).Select(x => x as TTTRoleButton);
        
            IEnumerable<TTTRoleButton> applicableButtons = roleButtons.Where(x => x.Role.ToLower() == "traitor");
        
            player.ClientStoreRoleButton(To.Single(ConsoleSystem.Caller), applicableButtons.Select(x => x.PackageData()).ToArray(), );
        }
        */

        //Handle client telling server to activate a specific button
        [ServerCmd]
        public static void ActivateRoleButton(int networkIdent)
        {
            TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;
            TTTRoleButton button = (TTTRoleButton) FindByIndex(networkIdent);

            if (button == null)
            {
                Log.Warning($"Server received call for null role button with network id `{networkIdent}`.");
                return;
            }

            if (player == null)
            {
                Log.Warning("Server received call from null player to activate role button.");
                return;
            }

            if (button.CanUse())
            {
                button.Press(player);
            }
        }

        //Client keybinding for activating button within focus.
        [ClientCmd("+ttt_activate_rb")]
        public static void StartRoleButtonActivate()
        {
            if (Local.Pawn is not TTTPlayer player || FocusedButton == null)
            {
                return;
            }

            //Double check all of our data that initially set `FocusedButton` to make sure nothing has changed or any fuckery is about.
            if (FocusedButton.IsLengthWithinCamerasFocus() && FocusedButton.IsUsable(player))
            {
                ActivateRoleButton(FocusedButton.Data.NetworkIdent);
            }
        }

        [ClientCmd("-ttt_activate_rb")]
        public static void EndRoleButtonActivate()
        {
        }
    }
}
