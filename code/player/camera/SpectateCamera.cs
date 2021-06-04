using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTTReborn.Player.Camera
{
    public partial class SpectateCamera : Sandbox.Camera
    {
        [Net, Predicted] public TimeSince TimeSinceDied { get; set; }
        [Net, Predicted] public Vector3 DeathPosition { get; set; }

        public TTTPlayer TargetPlayer { get; set; }

        private Vector3 _focusPoint;
        public int _targetIdx;

        public override void Activated()
        {
            base.Activated();

            _focusPoint = CurrentView.Position - GetViewOffset();

            FieldOfView = 70;
        }

        public override void Update()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            // TODO: Rework spectate camera logic.
            if (TargetPlayer == null || !TargetPlayer.IsValid() || Local.Client.Input.Pressed(InputButton.Attack1))
            {
                List<TTTPlayer> players = Client.All.ToList().ConvertAll(p => p.Pawn as TTTPlayer);

                if (players.Count > 0)
                {
                    if (++_targetIdx >= players.Count)
                        _targetIdx = 0;

                    TargetPlayer = players[_targetIdx];
                }
            }

            _focusPoint = Vector3.Lerp(_focusPoint, GetSpectatePoint(), Time.Delta * 5.0f);

            Pos = _focusPoint + GetViewOffset();
            Rot = player.EyeRot;

            FieldOfView = FieldOfView.LerpTo(50, Time.Delta * 3.0f);
            Viewer = null;
        }

        private Vector3 GetSpectatePoint()
        {
            if (Local.Pawn is not TTTPlayer)
                return DeathPosition;

            if (TargetPlayer == null || !TargetPlayer.IsValid() || TimeSinceDied < 3)
                return DeathPosition;

            return TargetPlayer.EyePos;
        }

        private Vector3 GetViewOffset()
        {
            if (Local.Pawn is not TTTPlayer player)
                return Vector3.Zero;

            return player.EyeRot.Forward * -150 + Vector3.Up * 10;
        }
    }

}
