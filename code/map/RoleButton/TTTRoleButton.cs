using Sandbox;

using TTTReborn.Player;
using TTTReborn.Rounds;

namespace TTTReborn.Map
{
    [Library("ttt_role_button", Description = "Used to provide an onscreen button for a role to activate.")]
    public partial class TTTRoleButton : Entity
    {
        [Property("Role")]
        public string Role { get; set; } = "Traitor";

        [Property("Description")]
        public string Description { get; set; }

        [Property("Range")]
        public int Range { get; set; } = 1024;

        [Property("Remove On Press")]
        public bool RemoveOnPress { get; set; } = false;

        [Property("Locked")]
        public bool Locked { get; set; } = false;

        [Property("Delay")]
        public float Delay { get; set; } = 0.0f;


    }

}
