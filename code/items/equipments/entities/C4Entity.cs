using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("entity_c4")]
    [Precached("models/entities/c4.vmdl", "particles/explosion_fireball.vpcf")]
    [Hammer.Skip]
    public partial class C4Entity : Prop, IEntityHint
    {
        public struct C4Preset
        {
            public int Timer;
            public int Wires;
        }

        public static readonly C4Preset[] TimerPresets =
        {
            new C4Preset
            {
                Timer = 60,
                Wires = 1
            },

            new C4Preset
            {
                Timer = 120,
                Wires = 2
            },

            new C4Preset
            {
                Timer = 180,
                Wires = 3
            },

            new C4Preset
            {
                Timer = 220,
                Wires = 4
            },

            new C4Preset
            {
                Timer = 250,
                Wires = 6
            },

            new C4Preset
            {
                Timer = 300,
                Wires = 8
            },
        };

        public override string ModelPath => "models/entities/c4.vmdl";

        [Net]
        public int AttachedBone { get; set; } = -1; // Defaults to -1, which indicates no bone attached as this value will not always be set.

        [Net, Change]
        public bool IsArmed { get; set; } = false;

        [Net]
        public C4Preset CurrentPreset { get; set; } = TimerPresets[0];

        // Timer display on C4 entity.
        private Sandbox.UI.WorldPanel TimerDisplay;
        private Sandbox.UI.Label TimerDisplayLabel;
        private bool CreatedDisplay = false;

        private const int BOMB_RADIUS = 1024;
        private const int BOMB_DAMAGE = 300;
        private const int BOMB_FORCE = 5;

        private const string EMPTY_TIMER = "--:--";

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        }

        public override void Simulate(Client cl)
        {
            if (IsClient && !CreatedDisplay)
            {
                TimerDisplay = new();
                CreatedDisplay = true;

                TimerDisplay.PanelBounds = new Rect(-120, 0, 140, 80);

                TimerDisplay.AddClass("c4worldtimer");
                TimerDisplay.StyleSheet.Add(Sandbox.UI.StyleSheet.FromFile("/ui/alivehud/c4/C4WorldTimer.scss"));

                TimerDisplayLabel = TimerDisplay.Add.Label();
                TimerDisplayLabel.Text = EMPTY_TIMER;
            }

            base.Simulate(cl);
        }

        [Event.Frame]
        private void UpdateDisplayTransform()
        {
            if (!CreatedDisplay)
            {
                return;
            }

            TimerDisplay.Transform = GetAttachment("timer") ?? Transform;
            TimerDisplay.WorldScale = 0.5f;
        }

        public void TryDisarm()
        {
            // Add a wire minigame in here later
            // For now, if you randomly roll the wrong wire the bomb explodes
            if (Utils.RNG.Next(CurrentPreset.Wires) != 0)
            {
                _ = Explode();

                return;
            }

            IsArmed = false;

            ClientUpdateTimer(EMPTY_TIMER);
        }

        private async void StartTimer()
        {
            IsArmed = true;

            int timeRemaining = CurrentPreset.Timer + 1;

            while (timeRemaining > 0 && IsArmed)
            {
                try
                {
                    if (timeRemaining <= 60)
                    {
                        Sound.FromEntity("c4-beep", this)
                            .SetVolume(0.05f);
                    }

                    timeRemaining -= 1;

                    TimeSpan span = TimeSpan.FromSeconds(timeRemaining);
                    string timerString = span.ToString("mm\\:ss");

                    ClientUpdateTimer(timerString);

                    await GameTask.DelaySeconds(1);
                }
                catch (Exception e)
                {
                    if (e.Message.Trim() == "A task was canceled.")
                    {
                        return;
                    }

                    Log.Error($"[TASK] {e.Message}: {e.StackTrace}");
                }
            }

            if (IsArmed)
            {
                await Explode();
            }
        }

        protected override void OnDestroy()
        {
            ClientCloseC4Menu(this);

            TimerDisplay?.Delete();
            TimerDisplay = null;

            base.OnDestroy();
        }


        // Modified from Prop.cs to allow tweaking through code/cvar rather than having to go through model doc.
        private async Task Explode()
        {
            try
            {
                IsArmed = false;

                await Task.DelaySeconds(0.1f);

                Sound.FromWorld("rust_pumpshotgun.shootdouble", PhysicsBody.MassCenter);
                Particles.Create("particles/explosion_fireball.vpcf", PhysicsBody.MassCenter);

                Vector3 sourcePos = PhysicsBody.MassCenter;
                IEnumerable<Entity> overlaps = FindInSphere(sourcePos, BOMB_RADIUS);

                foreach (Entity overlap in overlaps)
                {
                    if (overlap is not ModelEntity ent || !ent.IsValid() || ent.LifeState != LifeState.Alive || !ent.PhysicsBody.IsValid() || ent.IsWorld)
                    {
                        continue;
                    }

                    Vector3 targetPos = ent.PhysicsBody.MassCenter;
                    float dist = Vector3.DistanceBetween(sourcePos, targetPos);

                    if (dist > BOMB_RADIUS)
                    {
                        continue;
                    }

                    TraceResult tr = Trace.Ray(sourcePos, targetPos)
                        .Ignore(this)
                        .WorldOnly()
                        .Run();

                    if (tr.Fraction < 1.0f)
                    {
                        continue;
                    }

                    float distanceMul = 1.0f - Math.Clamp(dist / BOMB_RADIUS, 0.0f, 1.0f);
                    float damage = BOMB_DAMAGE * distanceMul;
                    float force = (BOMB_FORCE * distanceMul) * ent.PhysicsBody.Mass;
                    Vector3 forceDir = (targetPos - sourcePos).Normal;

                    ent.TakeDamage(DamageInfo.Explosion(sourcePos, forceDir * force, damage)
                        .WithAttacker(this));
                }
            }
            catch (Exception e)
            {
                if (e.Message.Trim() == "A task was canceled.")
                {
                    return;
                }

                Log.Error($"[TASK] {e.Message}: {e.StackTrace}");
            }

            base.OnKilled();
        }

        [ClientRpc]
        public void ClientUpdateTimer(string timerString)
        {
            TimerDisplayLabel.Text = timerString;
        }

        [ServerCmd]
        public static void Arm(int c4EntityIdent, int presetIndex)
        {
            if (FindByIndex(c4EntityIdent) is not C4Entity { IsArmed: false } c4Entity || c4Entity.Transform.Position.Distance(ConsoleSystem.Caller.Pawn.Position) > 100f)
            {
                return;
            }

            c4Entity.CurrentPreset = TimerPresets[presetIndex];
            c4Entity.StartTimer();
        }

        [ServerCmd]
        public static void Delete(int c4EntityIdent)
        {
            if (FindByIndex(c4EntityIdent) is C4Entity c4Entity && !c4Entity.IsArmed)
            {
                c4Entity.Delete();
            }
        }

        [ServerCmd]
        public static void PickUp(int c4EntityIdent, int playerIdent)
        {
            if (FindByIndex(c4EntityIdent) is C4Entity c4Entity && !c4Entity.IsArmed)
            {
                if (FindByIndex(playerIdent) is TTTPlayer player && player.Inventory.TryAdd(new C4Equipment()))
                {
                    c4Entity.Delete();
                }
            }
        }

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new(IsArmed ? "C4_DEFUSE" : "C4_ARM", new object[] { Input.GetKeyWithBinding("+iv_use").ToUpper() });

        public bool CanHint(TTTPlayer client)
        {
            return true;
        }

        public EntityHintPanel DisplayHint(TTTPlayer client)
        {
            return new Hint(TextOnTick);
        }

        public void Tick(TTTPlayer player)
        {
            if (IsClient)
            {
                return;
            }

            if (player.LifeState != LifeState.Alive)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    if (IsArmed)
                    {
                        TryDisarm();
                    }
                    else
                    {
                        ClientOpenC4Menu(To.Single(player), this);
                    }
                }
            }
        }

        public void OnIsArmedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                CloseC4Menu(this);
            }
        }

        [ClientRpc]
        public void ClientOpenC4Menu(C4Entity c4Entity)
        {
            if (c4Entity == null || !c4Entity.IsValid)
            {
                return;
            }

            C4Arm.Instance?.Open(c4Entity);
        }

        [ClientRpc]
        public void ClientCloseC4Menu(C4Entity c4Entity)
        {
            Host.AssertClient();

            CloseC4Menu(c4Entity);
        }

        private static void CloseC4Menu(C4Entity c4Entity)
        {
            if (C4Arm.Instance == null || c4Entity == null || !c4Entity.IsValid || C4Arm.Instance.Entity != c4Entity)
            {
                return;
            }

            C4Arm.Instance.Enabled(false);
        }
    }
}
