using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player.Camera;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private int _targetIdx = 0;

        [Event("tttreborn.player.died")]
        private static void OnPlayerDied(TTTPlayer deadPlayer)
        {
            if (!Host.IsClient || Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (player.IsSpectatingPlayer && player.CurrentPlayer == deadPlayer)
            {
                player.UpdateObservatedPlayer();
            }
        }

        public void UpdateObservatedPlayer()
        {
            TTTPlayer oldObservatedPlayer = CurrentPlayer;

            CurrentPlayer = null;

            List<TTTPlayer> players = Utils.GetAlivePlayers();

            if (players.Count > 0)
            {
                if (++_targetIdx >= players.Count)
                {
                    _targetIdx = 0;
                }

                CurrentPlayer = players[_targetIdx];
            }

            if (Camera is IObservationCamera camera)
            {
                camera.OnUpdateObservatedPlayer(oldObservatedPlayer, CurrentPlayer);
            }
        }
    }
}
