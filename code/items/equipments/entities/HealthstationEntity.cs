using System;

using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("entity_healthstation")]
    [Precached("models/entities/healthstation.vmdl")]
    public partial class HealthstationEntity : Prop, IEntityHint
    {
        [Net]
        public float StoredHealth { get; set; } = 200f; // This number technically has to be a float for the methods to work, but it should stay a whole number the entire time.

        public override string ModelPath => "models/entities/healthstation.vmdl";

        private RealTimeUntil NextHeal = 0;

        private const int HEALAMOUNT = 1;
        private const int HEALFREQUENCY = 1; // seconds
        private const int DELAYIFFAILED = 2; // Multiplied by HealFrequency if HealthPlayer returns false

        public override void Spawn()
        {
            base.Spawn();

            SetModel(ModelPath);
            SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        }

        private bool HealPlayer(TTTPlayer player)
        {
            float healthNeeded = player.MaxHealth - player.Health;

            if (StoredHealth > 0 && healthNeeded > 0)
            {
                float healAmount = Math.Min(HEALAMOUNT, healthNeeded);

                player.SetHealth(player.Health + healAmount);

                StoredHealth -= healAmount;

                return true;
            }

            return false;
        }

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new("HEALTH_STATION", new object[] { Input.GetKeyWithBinding("+iv_use").ToUpper(), $"{StoredHealth}" });

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
                if (Input.Down(InputButton.Use))
                {
                    if (player.Health < player.MaxHealth && NextHeal <= 0)
                    {
                        NextHeal = HealPlayer(player) ? HEALFREQUENCY : HEALFREQUENCY * DELAYIFFAILED;
                    }
                }
            }
        }
    }
}
