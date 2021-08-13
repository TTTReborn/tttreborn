using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Map;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public static List<TTTRoleButton> RoleButtons = new();

        public void SendRoleButtonsToClient() //Called Serverside when Player is assigned role.
        {
            IEnumerable<TTTRoleButton> roleButtons = All.Where(x => x.GetType() == typeof(TTTRoleButton)).Select(x => x as TTTRoleButton);

            List<TTTRoleButton> applicableButtons = roleButtons.Where(x => x.Role.ToLower() == Role.Name.ToLower()).ToList();

            ClientStoreRoleButton(To.Single(Owner), applicableButtons.Select(x => x.Position).ToArray());
        }

        [ClientRpc]
        public static void ClientStoreRoleButton(Vector3[] button)
        {
            //Handle logic clientside.
        }
    }
}
