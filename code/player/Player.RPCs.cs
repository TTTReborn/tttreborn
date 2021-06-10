using Sandbox;

namespace TTTReborn.Player
{
    partial class TTTPlayer
    {
        [ClientRpc]
        public void OnPlayerDied()
        {
            Event.Run("tttreborn.player.died");
        }

        [ClientRpc]
        public void OnPlayerSpawned()
        {
            Event.Run("tttreborn.player.spawned");
        }
    }
}
