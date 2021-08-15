using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_c4_ent"), Hammer.Skip]
    public partial class C4Entity : Prop, IUse
    {
        private string ModelPath => "models/entities/c4.vmdl";

        [Net]
        public int AttachedBone { get; set; } = -1; //Defaults to -1, which indicates no bone attached as this value will not always be set.

        [Net]
        public bool IsArmed { get; set; } = false;

        [Net]
        public C4State State { get; set; } = C4State.Unarmed;

        //Timer display on C4 entity.
        private WorldPanel BombDisplay;
        private bool CreatedDisplay = false;

        private const int BOMB_RADIUS = 1024;
        private const int BOMB_DAMAGE = 500;
        private const int BOMB_FORCE = 50;

        public override void Spawn()
        {
            base.Spawn();
            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        }

        public bool OnUse(Entity user)
        {
            TTTPlayer player = user as TTTPlayer;

            player.ClientOpenC4Control(State, this);

            return false; //Only fire once
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
                    BombDisplay.Transform = GetAttachment("timer") ?? Transform;
                    BombDisplay.WorldScale = 0.1f;

                    return;
                }
                else
                {
                    BombDisplay = new WorldPanel();
                    CreatedDisplay = true;

                    BombDisplay.AddClass("c4display");
                    BombDisplay.StyleSheet.Add(StyleSheet.FromFile("/ui/alivehud/c4/c4Display.scss"));

                    var label = BombDisplay.AddChild<Label>();
                    label.Text = "00:00";
                }
            }
            base.Simulate(cl);
        }

        public override void TakeDamage(DamageInfo info)
        {
            //TODO: Move this functionality to cvar.
            //If player or corpse with C4 attached is shot, explode.
            if (Parent is TTTPlayer || Parent is PlayerCorpse && (info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet)
            {
                _ = OnExplosion();
            }
        }

        //Modified from Prop.cs to allow tweaking through code/cvar rather than having to go through model doc.
        private async Task OnExplosion()
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
    }

    public enum C4State
    {
        Unarmed, //Shows initial timer
        Armed, //Shows wire cutting minigame
        Disarmed //Shows completed wire cutting minigame and "DISARMED" text. 
    }
}
