using Sandbox;

namespace TTTReborn.Player.Camera
{
    public partial class FirstPersonSpectatorCamera : Sandbox.Camera, IObservationCamera
    {
        private const float SMOOTH_SPEED = 25f;

        public override void Activated()
        {
            base.Activated();

            FieldOfView = 80;
        }

        public override void Deactivated()
        {
            base.Deactivated();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            player.CurrentPlayer.RenderColor = Color.White;
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

                Position = player.CurrentPlayer.EyePos;
                Rotation = player.CurrentPlayer.EyeRot;
            }
            else
            {
                Position = Vector3.Lerp(Position, player.CurrentPlayer.EyePos, SMOOTH_SPEED * Time.Delta);
                Rotation = Rotation.Slerp(Rotation, player.CurrentPlayer.EyeRot, SMOOTH_SPEED * Time.Delta);
            }
        }

        public void OnUpdateObservatedPlayer(TTTPlayer oldObservatedPlayer, TTTPlayer newObservatedPlayer)
        {
            if (oldObservatedPlayer != null)
            {
                oldObservatedPlayer.RenderColor = Color.White;
            }

            if (newObservatedPlayer != null)
            {
                newObservatedPlayer.RenderColor = Color.Transparent;
            }
        }
    }
}
