using System.Collections.Generic;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Camera;

namespace TTTReborn
{
    public partial class Player
    {
        private Player _spectatingPlayer;
        public Player CurrentPlayer
        {
            get => _spectatingPlayer ?? this;
            set
            {
                _spectatingPlayer = value == this ? null : value;

                Event.Run(TTTEvent.Player.Spectating.CHANGE, this);
            }
        }

        public bool IsSpectatingPlayer
        {
            get => _spectatingPlayer != null;
        }

        public bool IsSpectator
        {
            get => CameraMode is IObservationCamera;
        }

        private int _targetIdx = 0;

        [Event(TTTEvent.Player.DIED)]
        private static void OnPlayerDied(Player deadPlayer)
        {
            if (!Host.IsClient || Local.Pawn is not Player player)
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
            Player oldObservatedPlayer = CurrentPlayer;

            CurrentPlayer = null;

            List<Player> players = Utils.GetAlivePlayers();

            if (players.Count > 0)
            {
                if (++_targetIdx >= players.Count)
                {
                    _targetIdx = 0;
                }

                CurrentPlayer = players[_targetIdx];
            }

            if (CameraMode is IObservationCamera camera)
            {
                camera.OnUpdateObservatedPlayer(oldObservatedPlayer, CurrentPlayer);
            }
        }

        public void MakeSpectator(bool useRagdollCamera = true)
        {
            EnableAllCollisions = false;
            EnableDrawing = false;
            Controller = null;
            CameraMode = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();
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

                if (!Client.GetValue("forcedspectator", false))
                {
                    Client.SetValue("forcedspectator", true);
                }
            }
        }
    }
}
