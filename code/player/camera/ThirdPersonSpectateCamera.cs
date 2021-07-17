using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
namespace TTTReborn.Player.Camera
{
    public partial class ThirdPersonSpectateCamera : Sandbox.Camera
    {
        private Vector3 DefaultPosition { get; set; }

        private TTTPlayer TargetPlayer { get; set; }

        private const float LERP_MODE = 0;
        private const int FIELD_OF_VIEW_OVERRIDE = 70;
        private const int CAMERA_DISTANCE = 120;

        private Rotation _targetRot;
        private Vector3 _targetPos;
        private Angles _lookAngles;
        private int _targetIdx;

        public override void Activated()
        {
            base.Activated();

            Rot = CurrentView.Rotation;

            FieldOfView = FIELD_OF_VIEW_OVERRIDE;
        }

        public override void Update()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (TargetPlayer == null || !TargetPlayer.IsValid() || Input.Pressed(InputButton.Attack1))
            {
                List<TTTPlayer> players = Utils.GetAlivePlayers();

                if (players.Count > 0)
                {
                    if (++_targetIdx >= players.Count)
                    {
                        _targetIdx = 0;
                    }

                    TargetPlayer = players[_targetIdx];
                }
            }

            _targetRot = Rotation.From(_lookAngles);
            Rot = Rotation.Slerp(Rot, _targetRot, 10 * RealTime.Delta * (1 - LERP_MODE));

            _targetPos = GetSpectatePoint() + Rot.Forward * - CAMERA_DISTANCE;
            Pos = _targetPos;
        }

        private Vector3 GetSpectatePoint()
        {
            if (Local.Pawn is not TTTPlayer)
            {
                return DefaultPosition;
            }

            if (TargetPlayer == null || !TargetPlayer.IsValid())
            {
                return DefaultPosition;
            }

            return TargetPlayer.EyePos;
        }

        public override void BuildInput(InputBuilder input)
        {
            _lookAngles += input.AnalogLook * (FIELD_OF_VIEW_OVERRIDE / 80.0f);
            _lookAngles.roll = 0;

            base.BuildInput(input);
        }
    }
}
