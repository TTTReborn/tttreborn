using System;
using System.ComponentModel.DataAnnotations;

using Sandbox;

using TTTReborn.Entities;
using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_entity_healthstation")]
    [Precached("models/entities/healthstation.vmdl")]
    [Display(Name = "Healthstation", GroupName = "Equipments")]
    public partial class HealthstationEntity : Prop, IEntityHint
    {
        public const float MAX_HEALTH = 200f;

        [Net]
        public float StoredHealth { get; set; } = MAX_HEALTH; // This number technically has to be a float for the methods to work, but it should stay a whole number the entire time.

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

        private bool HealPlayer(Player player)
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

        public TranslationData[] TextOnTick => new TranslationData[] { new("EQUIPMENT.HEALTHSTATION.USE", $"{StoredHealth} / {MAX_HEALTH}") };

        public bool CanHint(Player client) => true;

        public EntityHintPanel DisplayHint(Player client) => new GlyphHint(new GlyphHintData[] { new(TextOnTick[0], InputButton.Use) });

        public void HintTick(Player player)
        {
            if (Game.IsClient || player.LifeState != LifeState.Alive)
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
