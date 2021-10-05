using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_c4_ent"), Hammer.Skip]
    public partial class C4Entity : Prop, IUse
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
                Timer = 150,
                Wires = 2
            },

            new C4Preset
            {
                Timer = 300,
                Wires = 4
            }
        };

        private string ModelPath => "models/entities/c4.vmdl";

        [Net]
        public int AttachedBone { get; set; } = -1; //Defaults to -1, which indicates no bone attached as this value will not always be set.

        [Net]
        public bool IsArmed { get; set; } = false;

        [Net]
        public C4Preset CurrentPreset { get; set; } = TimerPresets[0];

        //Timer display on C4 entity.
        private WorldPanel TimerDisplay;
        private Label TimerDisplayLabel;
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

        public bool OnUse(Entity user)
        {
            TTTPlayer player = user as TTTPlayer;

            if (IsArmed)
            {
                TryDisarm();
            }
            else
            {
                player.ClientOpenC4Menu(To.Single(player), this);
            }

            return false;
        }

        public void UpdateTimerDisplay(string timerString)
        {
            TimerDisplayLabel.Text = timerString;
        }

        public bool IsUsable(Entity user) => user is TTTPlayer;

        public override void Simulate(Client cl)
        {
            if (IsClient)
            {
                if (!CreatedDisplay)
                {
                    TimerDisplay = new WorldPanel();
                    CreatedDisplay = true;

                    TimerDisplay.AddClass("c4worldtimer");
                    TimerDisplay.StyleSheet.Add(StyleSheet.FromFile("/ui/alivehud/c4/C4WorldTimer.scss"));

                    TimerDisplay.PanelBounds = new Rect(0, 0, 140, 80);

                    TimerDisplayLabel = TimerDisplay.AddChild<Label>();
                    TimerDisplayLabel.Text = EMPTY_TIMER;
                }
            }

            base.Simulate(cl);
        }

        [Event.Frame]
        private void UpdateDisplayTransform()
        {
            if (CreatedDisplay)
            {
                TimerDisplay.Transform = GetAttachment("timer") ?? Transform;
                TimerDisplay.WorldScale = 0.5f;

                return;
            }
        }

        public void TryDisarm()
        {
            // Add a wire minigame in here later
            // For now, if you randomly roll the wrong wire the bomb explodes
            int disarmRoll = new Random().Next(1, CurrentPreset.Wires + 1);
            if (disarmRoll != 1)
            {
                _ = Explode();
                return;
            }

            IsArmed = false;

            ClientUpdateTimer(EMPTY_TIMER);
        }

        public async void StartTimer()
        {
            IsArmed = true;

            var timeRemaining = CurrentPreset.Timer + 1;

            while (timeRemaining > 0 && IsArmed)
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

            if (IsArmed)
            {
                await Explode();
            }
        }

        protected override void OnDestroy()
        {
            TimerDisplay?.Delete();
            TimerDisplay = null;

            base.OnDestroy();
        }


        //Modified from Prop.cs to allow tweaking through code/cvar rather than having to go through model doc.
        private async Task Explode()
        {
            IsArmed = false;

            await Task.DelaySeconds(0.1f);

            Sound.FromWorld("rust_pumpshotgun.shootdouble", PhysicsBody.MassCenter);
            Particles.Create("particles/explosion_fireball.vpcf", PhysicsBody.MassCenter);

            Vector3 sourcePos = PhysicsBody.MassCenter;
            IEnumerable<Entity> overlaps = Physics.GetEntitiesInSphere(sourcePos, BOMB_RADIUS);

            foreach (Entity overlap in overlaps)
            {
                if (overlap is not ModelEntity ent || !ent.IsValid())
                    continue;

                if (ent.LifeState != LifeState.Alive)
                    continue;

                if (!ent.PhysicsBody.IsValid())
                    continue;

                if (ent.IsWorld)
                    continue;

                Vector3 targetPos = ent.PhysicsBody.MassCenter;

                float dist = Vector3.DistanceBetween(sourcePos, targetPos);
                if (dist > BOMB_RADIUS)
                    continue;

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

            base.OnKilled();
        }

        [ClientRpc]
        public void ClientUpdateTimer(string timerString)
        {
            TimerDisplayLabel.Text = timerString;
        }

        [ServerCmd]
        public static void Arm(int c4EntityIdent)
        {
            Entity entity = FindByIndex(c4EntityIdent);

            if (entity != null && entity is C4Entity c4Entity && !c4Entity.IsArmed)
            {
                c4Entity.StartTimer();
            }
        }

        [ServerCmd]
        public static void Delete(int c4EntityIdent)
        {
            Entity entity = FindByIndex(c4EntityIdent);

            if (entity != null && entity is C4Entity c4Entity)
            {
                c4Entity.Delete();
            }
        }

        [ServerCmd]
        public static void PickUp(int c4EntityIdent, int playerIdent)
        {
            Entity player = FindByIndex(playerIdent);

            if (player != null && player is TTTPlayer)
            {
                if ((player.Inventory as Inventory).TryAdd(new C4Equipment()))
                {
                    Delete(c4EntityIdent);
                }
            }
        }

        [ServerCmd]
        public static void SetPreset(int c4EntityIdent, int preset)
        {
            Entity entity = FindByIndex(c4EntityIdent);

            if (entity != null && entity is C4Entity c4Entity)
            {
                c4Entity.CurrentPreset = TimerPresets[preset];
            }
        }
    }
}
