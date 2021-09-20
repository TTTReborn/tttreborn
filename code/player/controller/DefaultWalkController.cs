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
        public static bool IsSprintEnabled = false;

        public const float MAX_STAMINA = 100f;
        public const float MAX_SPRINT_SPEED = 400f;
        public const float STAMINA_LOSS_PER_SECOND = 25f;
        public const float STAMINA_LOSS_PER_SPRINT_JUMP = 30f;
        public const float STAMINA_GAIN_PER_SECOND = 10f;

        public const float FALL_DAMAGE_VELOCITY = 550f;
        public const float FALL_DAMAGE_SCALE = 0.25f;

        public const float MAX_BREATH = 100f;
        public const float BREATH_LOSS_PER_SECOND = 10f;
        public const float BREATH_GAIN_PER_SECOND = 50f;
        public const float DROWN_DAMAGE_PER_SECOND = 20f;

        [Net, Predicted] public float Stamina { get; set; } = 100f;
        [Net] public float Breath { get; set; } = 100f;
        public bool IsUnderwater { get; set; } = false;

        private float _fallVelocity;

        public DefaultWalkController() : base()
        {
            GroundFriction = 8f;
            DefaultSpeed = 175f;
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

            if (IsSprintEnabled)
            {
                if (Input.Down(InputButton.Run) && Velocity.Length >= SprintSpeed * 0.8f && player.GroundEntity.IsValid())
                {
                    Stamina = MathF.Max(Stamina - STAMINA_LOSS_PER_SECOND * Time.Delta, 0f);

                    if (Input.Pressed(InputButton.Jump))
                    {
                        Stamina = MathF.Max(Stamina - STAMINA_LOSS_PER_SPRINT_JUMP, 0f);
                    }
                }
                else
                {
                    Stamina = MathF.Min(Stamina + STAMINA_GAIN_PER_SECOND * Time.Delta, MAX_STAMINA);
                }

                SprintSpeed = (MAX_SPRINT_SPEED - DefaultSpeed) / MAX_STAMINA * Stamina + DefaultSpeed;
            }
            #endregion

            #region Drowning
            IsUnderwater = Pawn.WaterLevel.Fraction == 1f;

            if (IsUnderwater)
            {
                Breath = MathF.Max(Breath - BREATH_LOSS_PER_SECOND * Time.Delta, 0f);
            }
            else
            {
                Breath = MathF.Min(Breath + BREATH_GAIN_PER_SECOND * Time.Delta, MAX_BREATH);
            }

            if (Host.IsServer && Breath == 0f)
            {
                using (Prediction.Off())
                {
                    DamageInfo damageInfo = new DamageInfo
                    {
                        Attacker = Pawn,
                        Flags = DamageFlags.Drown,
                        HitboxIndex = (int) HitboxIndex.Head,
                        Position = Position,
                        Damage = MathF.Max(DROWN_DAMAGE_PER_SECOND * Time.Delta, 0f)
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
            if (Host.IsServer && trace.Hit && _fallVelocity < -FALL_DAMAGE_VELOCITY)
            {
                using (Prediction.Off())
                {
                    DamageInfo damageInfo = new()
                    {
                        Attacker = Pawn,
                        Flags = DamageFlags.Fall,
                        HitboxIndex = (int) HitboxIndex.LeftFoot,
                        Position = Position,
                        Damage = (MathF.Abs(_fallVelocity) - FALL_DAMAGE_VELOCITY) * FALL_DAMAGE_SCALE
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
