using System;

using Sandbox;

namespace TTTReborn.Settings
{
    using Player;

    public partial class ServerSettings
    {
        public bool IsSprintEnabled
        {
            get => DefaultWalkController.IsSprintEnabled;
            set
            {
                if (DefaultWalkController.IsSprintEnabled == value)
                {
                    return;
                }

                DefaultWalkController.SetSprintEnabled(value);
            }
        }
    }
}

namespace TTTReborn.Player
{
    public partial class DefaultWalkController : WalkController
    {
        public static bool IsSprintEnabled { get; set; } = false;

        public float MaxSprintSpeed = 300f;
        public float StaminaLossPerSecond = 30f;
        public float StaminaGainPerSecond = 25f;
        public float FallDamageVelocity = 550f;
        public float FallDamageScale = 0.25f;
        public bool IsUnderwater = false;
        public float DurationUnderwaterUntilDamage = 15f;
        public float DrownDamagePerSecond = 10f;
        public TimeSince TimeSinceUnderwater = 0f;

        private float _fallVelocity;


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

            IsUnderwater = Pawn.WaterLevel.Fraction == 1f;

            if (!IsUnderwater)
            {
                TimeSinceUnderwater = 0f;
            }
            else if (Host.IsServer && TimeSinceUnderwater > DurationUnderwaterUntilDamage)
            {
                using (Prediction.Off())
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        Attacker = Pawn,
                        Flags = DamageFlags.Drown,
                        HitboxIndex = (int) HitboxIndex.Head,
                        Position = Position,
                        Damage = MathF.Max(DrownDamagePerSecond * Time.Delta, 0f)
                    };

                    Pawn.TakeDamage(damageInfo);
                }
            }

            OnPreTickMove();

            base.Simulate();
        }

        public void OnPreTickMove()
        {
            _fallVelocity = Velocity.z;
        }

        public override void CategorizePosition(bool stayOnGround)
        {
            base.CategorizePosition(stayOnGround);

            Vector3 point = Position - Vector3.Up * 2;

            if (GroundEntity != null || stayOnGround)
            {
                point.z -= StepSize;
            }

            TraceResult pm = TraceBBox(Position, point, 4.0f);

            OnPostCategorizePosition(stayOnGround, pm);
        }

        public virtual void OnPostCategorizePosition(bool stayOnGround, TraceResult trace)
        {
            if (Host.IsServer && trace.Hit && _fallVelocity < -FallDamageVelocity)
            {
                using (Prediction.Off())
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        Attacker = Pawn,
                        Flags = DamageFlags.Fall,
                        HitboxIndex = (int) HitboxIndex.LeftFoot,
                        Position = Position,
                        Damage = (MathF.Abs(_fallVelocity) - FallDamageVelocity) * FallDamageScale
                    };

                    Pawn.TakeDamage(damageInfo);
                }
            }
        }

        public static void SetSprintEnabled(bool enabled)
        {
            IsSprintEnabled = enabled;

            ClientSendToggleSprint(IsSprintEnabled);
        }

        [ServerCmd(Name = "ttt_toggle_sprint", Help = "Toggles sprinting")]
        public static void ToggleSprinting()
        {
            if (!ConsoleSystem.Caller.HasPermission("ttt_toggle_sprint"))
            {
                return;
            }

            Settings.ServerSettings.Instance.IsSprintEnabled = !IsSprintEnabled;

            string text = IsSprintEnabled ? "enabled" : "disabled";

            Log.Info($"You {text} sprinting.");

            return;
        }

        [ClientRpc]
        public static void ClientSendToggleSprint(bool toggle)
        {
            IsSprintEnabled = toggle;
        }

        [Event("tttreborn.player.initialspawn")]
        public static void InitializeSprint()
        {
            ClientSendToggleSprint(IsSprintEnabled);
        }
    }
}
