using System;

using Sandbox;

using TTTReborn.Settings;

namespace TTTReborn.Settings
{
    public partial class ServerSettings
    {
        public Categories.Movement Movement { get; set; } = new Categories.Movement();
    }


    namespace Categories
    {
        public partial class Movement
        {
            public bool IsSprintEnabled { get; set; } = false;
        }
    }
}

namespace TTTReborn.Player
{
    public partial class DefaultWalkController : WalkController
    {
        public const float MAX_UNDERWATER_BREATH_SECONDS = 10f;

        public static bool IsSprintEnabled = false;

        public float MaxSprintSpeed = 300f;
        public float StaminaLossPerSecond = 30f;
        public float StaminaGainPerSecond = 25f;
        public float FallDamageVelocity = 550f;
        public float FallDamageScale = 0.25f;
        public bool IsUnderwater = false;
        public float UnderwaterBreathSeconds = 10f;
        public float DrownDamagePerSecond = 10f;

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

            #region Sprinting
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
            #endregion

            #region Drowning
            IsUnderwater = Pawn.WaterLevel.Fraction == 1f;

            UnderwaterBreathSeconds = Math.Clamp(UnderwaterBreathSeconds + Time.Delta * (IsUnderwater ? -1f : 1f), 0f, MAX_UNDERWATER_BREATH_SECONDS);

            if (Host.IsServer && UnderwaterBreathSeconds <= 0f)
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
            #endregion

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

        [Event("tttreborn.player.initialspawn")]
        [Event("tttreborn.settings.instance.change")]
        public static void InitializeSprint()
        {
            if (Host.IsServer)
            {
                bool enabled = ServerSettings.Instance.Movement.IsSprintEnabled;

                IsSprintEnabled = enabled;

                TTTPlayer.ClientSendToggleSprint(enabled);
            }
        }
    }

    public partial class TTTPlayer
    {
        [ServerCmd(Name = "ttt_toggle_sprint", Help = "Toggles sprinting")]
        public static void ToggleSprinting()
        {
            if (!ConsoleSystem.Caller.HasPermission("ttt_toggle_sprint"))
            {
                return;
            }

            DefaultWalkController.IsSprintEnabled = !DefaultWalkController.IsSprintEnabled;

            string text = DefaultWalkController.IsSprintEnabled ? "enabled" : "disabled";

            Log.Info($"You {text} sprinting.");

            return;
        }

        [ClientRpc]
        public static void ClientSendToggleSprint(bool toggle)
        {
            DefaultWalkController.IsSprintEnabled = toggle;
        }
    }
}
