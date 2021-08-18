using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_c4_ent"), Hammer.Skip]
    public partial class C4Entity : Prop, IUse
    {
        public static C4Preset[] TimerPresets =
        {
            new C4Preset
            {
                timer = 60,
                wires = 1
            },

            new C4Preset
            {
                timer = 150,
                wires = 3
            },

            new C4Preset
            {
                timer = 300,
                wires = 5
            }
        };

        private string ModelPath => "models/entities/c4.vmdl";

        [Net]
        public int AttachedBone { get; set; } = -1; //Defaults to -1, which indicates no bone attached as this value will not always be set.

        [Net]
        public bool IsArmed
        {
            get
            {
                return State == C4State.Armed;
            }
        }

        [Net]
        public C4State State { get; set; } = C4State.Unarmed;

        //Timer display on C4 entity.
        private WorldPanel TimerDisplay;
        private Label TimerDisplayLabel;
        private bool CreatedDisplay = false;

        private const int BOMB_RADIUS = 1024;
        private const int BOMB_DAMAGE = 300;
        private const int BOMB_FORCE = 5;

        private List<TTTPlayer> _currentUsers = new();

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        }

        public bool OnUse(Entity user)
        {
            TTTPlayer player = user as TTTPlayer;

            switch(State)
            {
                case C4State.Armed:
                    Disarm();
                    break;

                default:
                    player.ClientOpenC4Menu(this);
                    break;
            }

            _currentUsers.Add(player);

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
                if (CreatedDisplay)
                {
                    //No way to parent a world panel to an entity :(
                    //We need to find a better way of doing this.
                    //I heard supposedly you can use FrameSimulate, but I was getting weird duplicated results (think Windows XP window glitch)
                    TimerDisplay.Transform = GetAttachment("timer") ?? Transform;
                    TimerDisplay.WorldScale = 0.05f;

                    return;
                }
                else
                {
                    TimerDisplay = new WorldPanel();
                    CreatedDisplay = true;

                    TimerDisplay.AddClass("c4worldtimer");
                    TimerDisplay.StyleSheet.Add(StyleSheet.FromFile("/ui/alivehud/c4/C4WorldTimer.scss"));

                    TimerDisplayLabel = TimerDisplay.AddChild<Label>();
                    TimerDisplayLabel.Text = "--:--";
                }
            }

            // If the player moves away from the bomb, close the UI
            if (_currentUsers.Count > 0)
            {
                for (int i = _currentUsers.Count - 1; i >= 0; i--)
                {
                    if (Vector3.DistanceBetween(_currentUsers[i].Controller.Position, this.Position) > 100)
                    {
                        _currentUsers[i].ClientCloseC4Menu();
                        _currentUsers.Remove(_currentUsers[i]);
                    }
                }
            }

            Log.Info("Simulate!");

            base.Simulate(cl);
        }

        public void Disarm()
        {
            State = C4State.Disarmed;
            ClientUpdateTimer("--:--");
        }

        public async void StartTimer(C4TimerPreset preset)
        {
            State = C4State.Armed;

            var timeRemaining = TimerPresets[(int)preset].timer + 1;

            while (timeRemaining > 0 && IsArmed)
            {
                if (timeRemaining < 60)
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
        public static void Arm(int c4EntityIdent, C4TimerPreset preset)
        {
            var c4Entity = ((C4Entity) FindByIndex(c4EntityIdent));

            if (c4Entity.State != C4State.Armed)
            {
                c4Entity.StartTimer(preset);
            }
        }

        [ServerCmd]
        public static void Delete(int c4EntityIdent)
        {
            FindByIndex(c4EntityIdent).Delete();
        }

        [ServerCmd]
        public static void PickUp(int c4EntityIdent, int playerIdent)
        {
            var player = FindByIndex(playerIdent);

            if ((player.Inventory as Inventory).TryAdd(new C4Equipment()))
            {
                Delete(c4EntityIdent);
            }
        }
    }

    public enum C4State
    {
        Unarmed, //Shows initial timer
        Armed, //Shows wire cutting minigame
        Disarmed //Shows completed wire cutting minigame and "DISARMED" text. 
    }

    public enum C4TimerPreset
    {
        None = -1,
        Short,
        Medium,
        Long
    }

    public struct C4Preset
    {
        public int timer;
        public int wires;
    }
}
