using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [Library("ttt_healthkit_ent")]
    public partial class HealthkitEntity : Prop, IUse
    {
        [Net]
        public float StoredHealth { get; set; } = 200; //This number technically has to be a float for the methods to work, but it should stay a whole number the entire time.

        private string ModelPath => "models/entities/healthstation.vmdl";

        private RealTimeUntil NextHeal = 0;

        private const int HealAmount = 1;
        private const int HealFrequency = 2; //seconds
        private const int DelayIfFailed = 2; //Multiplied by HealFrequency if HealthPlayer returns false

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
            //CollisionGroup = CollisionGroup.Weapon; //Uncomment this if you want to be able to walk through the health kit.
        }

        private bool HealPlayer(TTTPlayer player)
        {
            if (StoredHealth > 0)
            {
                float healthNeeded = player.MaxHealth - player.Health;

                if (healthNeeded > 0)
                {
                    var healAmount = Math.Min(HealAmount, healthNeeded);

                    player.SetHealth(player.Health + healAmount);
                    StoredHealth -= healAmount;

                    //Event.Run("tttreborn.healthstation.healed", player, healAmount); //Would have liked to pass `this` as well, but apparently events are capped at 2 parameters?

                    //Play sounds here
                    //Store DNA data once that becomes a thing
                    return true;
                }
            }
            return false;
        }

        public bool OnUse(Entity user)
        {
            if (user is TTTPlayer player && NextHeal <= 0)
            {
                NextHeal = HealPlayer(player) ? HealFrequency : HealFrequency * DelayIfFailed;
            }

            return true;
        }

        public bool IsUsable(Entity user)
        {
            if (user is TTTPlayer player && player.Health < player.MaxHealth)
            {
                return true;
            }

            return false;
        }
    }
}
