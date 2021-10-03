using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Player.Camera;

namespace TTTReborn.UI
{
    public class Nameplate : EntityHintPanel
    {
        public TTTPlayer Player;

        private readonly Sandbox.UI.Panel _labelHolder;
        private readonly Sandbox.UI.Panel _nameHolder;
        private readonly Label _nameLabel;
        private readonly Label _damageIndicatorLabel;

        private struct HealthGroup
        {
            public string Title;
            public Color Color;
            public int MinHealth;

            public HealthGroup(string title, Color color, int minHealth)
            {
                Title = title;
                Color = color;
                MinHealth = minHealth;
            }
        }

        // Pay attention when adding new values! The highest health-based entry has to be the first item, etc.
        private HealthGroup[] HealthGroupList = new HealthGroup[]
        {
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 66),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44), 33),
            new HealthGroup("Near Death", Color.FromBytes(252, 42, 42), 0)
        };

        public Nameplate(TTTPlayer player) : base()
        {
            Player = player;

            StyleSheet.Load("/ui/generalhud/nameplate/Nameplate.scss");

            AddClass("text-shadow");

            _labelHolder = Add.Panel("label-holder");

            _nameHolder = _labelHolder.Add.Panel("name-holder");
            _nameLabel = _nameHolder.Add.Label("", "name");

            _damageIndicatorLabel = _labelHolder.Add.Label("", "damage-indicator");

            Enabled = false;
        }

        private HealthGroup GetHealthGroup(float health)
        {
            foreach (HealthGroup healthGroup in HealthGroupList)
            {
                if (health >= healthGroup.MinHealth)
                {
                    return healthGroup;
                }
            }

            return HealthGroupList[^1];
        }

        public override void UpdateHintPanel()
        {
            SetClass("fade-in", Enabled);

            // Network sync workaround
            if (Player.Health == 0 && Player.LifeState == LifeState.Alive)
            {
                _damageIndicatorLabel.Text = "";
            }
            else
            {
                float health = Player.Health / Player.MaxHealth * 100;
                HealthGroup healthGroup = GetHealthGroup(health);

                _damageIndicatorLabel.Style.FontColor = healthGroup.Color;
                _damageIndicatorLabel.Text = healthGroup.Title;
                _damageIndicatorLabel.Style.Dirty();
            }

            _nameLabel.Text = Player.Client?.Name ?? "";

            Style.Dirty();
        }
    }
}
