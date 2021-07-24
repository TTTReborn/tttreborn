using System.Collections.Generic;

using Sandbox;

namespace TTTReborn.Player.Camera
{
    public partial class FirstPersonSpectatorCamera : Sandbox.Camera, IObservationCamera
    {
        private Vector3 _lastPos;
        private Rotation _lastRot;

        private const float SMOOTH_SPEED = 25f;

        public override void Deactivated()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            _lastPos = Pos;
            _lastRot = Rot;

            player.CurrentPlayer.RenderAlpha = 1f;
            player.CurrentPlayer = null;
        }

        public override void Update()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (!player.IsSpectatingPlayer || Input.Pressed(InputButton.Attack1))
            {
                player.UpdateObservatedPlayer();

                Pos = player.CurrentPlayer.EyePos;
                Rot = player.CurrentPlayer.EyeRot;
            }
            else
            {
                Pos = Vector3.Lerp(_lastPos, player.CurrentPlayer.EyePos, SMOOTH_SPEED * Time.Delta);
                Rot = Rotation.Slerp(_lastRot, player.CurrentPlayer.EyeRot, SMOOTH_SPEED * Time.Delta);
            }

            _lastPos = Pos;
            _lastRot = Rot;

            FieldOfView = 80;
        }

        public void OnUpdateObservatedPlayer(TTTPlayer oldObservatedPlayer, TTTPlayer newObservatedPlayer)
        {
            if (oldObservatedPlayer != null)
            {
                oldObservatedPlayer.RenderAlpha = 1f;
            }

            if (newObservatedPlayer != null)
            {
                newObservatedPlayer.RenderAlpha = 0f;
            }
        }
    }
}
