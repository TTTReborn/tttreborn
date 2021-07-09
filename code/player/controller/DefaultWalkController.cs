using System;

using Sandbox;

namespace TTTReborn.Player
{
    public class DefaultWalkController : WalkController
    {
        public float MaxSprintSpeed = 300f;
        public float StaminaLossPerSecond = 30f;
        public float StaminaGainPerSecond = 35f;

        public DefaultWalkController() : base()
        {
            GroundFriction = 8f;
        }

        public override void Simulate()
        {
            if (Pawn is not TTTPlayer player)
            {
                base.Simulate();

                return;
            }

            if (Input.Down(InputButton.Run) && Velocity.Length >= SprintSpeed * 0.8f)
            {
                player.Stamina = MathF.Max(player.Stamina - StaminaLossPerSecond * Time.Delta, 0f);
            }
            else
            {
                player.Stamina = MathF.Min(player.Stamina + StaminaGainPerSecond * Time.Delta, 100f);
            }

            SprintSpeed = WalkSpeed + ((MaxSprintSpeed - WalkSpeed) / 100f * player.Stamina) + 40f;

            base.Simulate();
        }
    }
}
