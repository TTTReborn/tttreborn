using System;

using Sandbox;

namespace TTTReborn.Player
{
    public partial class DefaultWalkController : WalkController
    {
        [ServerVar]
        public static bool IsSprintEnabled { get; set; } = false;

        public float MaxSprintSpeed = 300f;
        public float StaminaLossPerSecond = 30f;
        public float StaminaGainPerSecond = 25f;

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

            SprintSpeed = DefaultSpeed;

            if (IsSprintEnabled && player.GroundEntity.IsValid())
            {
                if (Input.Down(InputButton.Run) && Velocity.Length >= SprintSpeed * 0.8f)
                {
                    player.Stamina = MathF.Max(player.Stamina - StaminaLossPerSecond * Time.Delta, 0f);
                }
                else
                {
                    player.Stamina = MathF.Min(player.Stamina + StaminaGainPerSecond * Time.Delta, player.MaxStamina);
                }

                SprintSpeed = (MaxSprintSpeed - DefaultSpeed) / player.MaxStamina * player.Stamina + DefaultSpeed;
            }

            base.Simulate();
        }

        [ServerCmd(Name = "ttt_toggle_sprint", Help = "Toggles sprinting")]
        public static void ToggleSprinting()
        {
            if (!ConsoleSystem.Caller.HasPermission("ttt_toggle_sprint"))
            {
                return;
            }

            IsSprintEnabled = !IsSprintEnabled;

            ClientSendToggleSprint(IsSprintEnabled);

            string text = IsSprintEnabled ? "enabled" : "disabled";

            Log.Info($"You {text} sprinting.");

            return;
        }

        [ClientRpc]
        public static void ClientSendToggleSprint(bool toggle)
        {
            IsSprintEnabled = toggle;
        }
    }
}
