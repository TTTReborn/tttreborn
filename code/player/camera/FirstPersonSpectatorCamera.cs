using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;

namespace TTTReborn.Player.Camera
{
    public partial class FirstPersonSpectatorCamera : Sandbox.Camera, IObservableCamera
    {
        public TTTPlayer TargetPlayer { get; set; }

        private int _targetIdx;
		private Vector3 _lastPos;
        private Rotation _lastRot;

		public override void Activated()
		{
			if (TargetPlayer == null)
            {
                return;
            }

			Pos = TargetPlayer.EyePos;
			Rot = TargetPlayer.EyeRot;

			_lastPos = Pos;
            _lastRot = Rot;

            TargetPlayer.RenderAlpha = 0f;
		}

        public override void Deactivated()
        {
            if (TargetPlayer == null || !TargetPlayer.IsValid())
            {
                return;
            }

            TargetPlayer.RenderAlpha = 1f;
        }

		public override void Update()
		{
            bool invalidTarget = TargetPlayer == null || !TargetPlayer.IsValid();

			if (invalidTarget || Input.Pressed(InputButton.Attack1))
            {
                if (!invalidTarget)
                {
                    TargetPlayer.RenderAlpha = 1f;
                }

                List<TTTPlayer> players = Utils.GetAlivePlayers();

                if (players.Count > 0)
                {
                    if (++_targetIdx >= players.Count)
                    {
                        _targetIdx = 0;
                    }

                    TargetPlayer = players[_targetIdx];
                }

                TargetPlayer.RenderAlpha = 0f;

                return;
            }

            using (Prediction.Off())
            {
                Vector3 eyePos = TargetPlayer.EyePos;
                Rotation eyeRot = TargetPlayer.EyeRot;

                Pos = Vector3.Lerp(_lastPos, eyePos, 20.0f * Time.Delta);
                Rot = Rotation.Lerp(_lastRot, eyeRot, 20.0f * Time.Delta);

                FieldOfView = 80;

                _lastPos = Pos;
                _lastRot = Rot;
            }
		}
    }
}
