using System;

using Sandbox;

using TTTReborn.Hints;
using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    [Library("ttt_healthstation_ent"), Hammer.Skip]
    public partial class HealthstationEntity : Prop, IUse, IEntityHint
    {
        [Net]
        public float StoredHealth { get; set; } = 200f; // This number technically has to be a float for the methods to work, but it should stay a whole number the entire time.

        private float _oldStoredHealth;

        private string ModelPath => "models/entities/healthstation.vmdl";

        private RealTimeUntil NextHeal = 0;

        private const int HEALAMOUNT = 1;
        private const int HEALFREQUENCY = 2; // seconds
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

        public bool OnUse(Entity user)
        {
            if (user is TTTPlayer player && NextHeal <= 0)
            {
                NextHeal = HealPlayer(player) ? HEALFREQUENCY : HEALFREQUENCY * DELAYIFFAILED;
            }

            return true;
        }

        public bool IsUsable(Entity user) => (user is TTTPlayer player && player.Health < player.MaxHealth);

        public bool CanHint(TTTPlayer player)
        {
            return true;
        }

        public EntityHintPanel DisplayHintPanel(TTTPlayer player)
        {
            _oldStoredHealth = StoredHealth;

            return new TranslationLabelHintPanel("HEALTH_STATION", Input.GetKeyWithBinding("+iv_use").ToUpper(), $"{_oldStoredHealth}");
        }

        public void UpdateHintPanel(EntityHintPanel entityHintPanel)
        {
            if (StoredHealth == _oldStoredHealth)
            {
                return;
            }

            _oldStoredHealth = StoredHealth;

            (entityHintPanel as TranslationLabelHintPanel).TranslationLabel.SetTranslation("HEALTH_STATION", Input.GetKeyWithBinding("+iv_use").ToUpper(), $"{_oldStoredHealth}");
        }
    }
}
