using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player.Camera;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private TTTPlayer _spectatingPlayer;
        public TTTPlayer CurrentPlayer
        {
            get => _spectatingPlayer ?? this;
            set
            {
                _spectatingPlayer = value == this ? null : value;

                Event.Run("tttreborn.player.spectating.change", this);
            }
        }

        public bool IsSpectatingPlayer
        {
            get => _spectatingPlayer != null;
        }

        public bool IsSpectator
        {
            get => (Camera is IObservationCamera);
        }

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

        public void MakeSpectator(bool useRagdollCamera = true)
        {
            EnableAllCollisions = false;
            EnableDrawing = false;
            Controller = null;
            Camera = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();
            LifeState = LifeState.Dead;
            Health = 0f;
            ShowFlashlight(false, false);
        }

        public void ToggleForcedSpectator()
        {
            IsForcedSpectator = !IsForcedSpectator;

            if (IsForcedSpectator && LifeState == LifeState.Alive)
            {
                MakeSpectator(false);
                OnKilled();

                if (!GetClientOwner().GetScore("forcedspectator", false))
                {
                    GetClientOwner().SetScore("forcedspectator", true);
                }
            }
        }
    }
}
