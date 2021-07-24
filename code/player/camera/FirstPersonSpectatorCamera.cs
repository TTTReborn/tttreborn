using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;

namespace TTTReborn.Player.Camera
{
    public partial class FirstPersonSpectatorCamera : Sandbox.Camera, IObservationCamera
    {
        private int _targetIdx;

        public override void Deactivated()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

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
                if (player.IsSpectatingPlayer)
                {
                    player.CurrentPlayer.RenderAlpha = 1f;
                }

                player.CurrentPlayer = null;

                List<TTTPlayer> players = Utils.GetAlivePlayers();

                if (players.Count > 0)
                {
                    if (++_targetIdx >= players.Count)
                    {
                        _targetIdx = 0;
                    }

                    player.CurrentPlayer = players[_targetIdx];
                    player.CurrentPlayer.RenderAlpha = 0f;
                }
            }

            Pos = player.CurrentPlayer.EyePos;
            Rot = player.CurrentPlayer.EyeRot;

            FieldOfView = 80;
        }
    }
}
