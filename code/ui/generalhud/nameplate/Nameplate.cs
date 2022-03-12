using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class Nameplate : EntityHintPanel
    {
        public TTTPlayer Player;

        private readonly Panel _labelHolder;
        private readonly Panel _nameHolder;
        private readonly Label _nameLabel;
        private readonly Label _damageIndicatorLabel;
        private readonly TranslationLabel _roleLabel;
        private Role _role;

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
        private readonly HealthGroup[] HealthGroupList = new[]
        {
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 66),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44), 33),
            new HealthGroup("Near Death", Color.FromBytes(252, 42, 42), 0)
        };

        public Nameplate(TTTPlayer player) : base()
        {
            Player = player;
            _role = player.Role;

            StyleSheet.Load("/ui/generalhud/nameplate/Nameplate.scss");

            AddClass("text-shadow");

            _labelHolder = Add.Panel("label-holder");

            _nameHolder = _labelHolder.Add.Panel("name-holder");
            _nameLabel = _nameHolder.Add.Label("", "name");

            _damageIndicatorLabel = _labelHolder.Add.Label("", "damage-indicator");

            _roleLabel = _labelHolder.Add.TranslationLabel(new TranslationData(_role.GetTranslationKey("NAME")), "role");
            _roleLabel.Style.FontColor = _role is NoneRole ? Color.White : _role.Color;
            _roleLabel.Style.TextShadow.Clear();

            this.Enabled(false);
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

        public override void UpdateHintPanel(TranslationData translationData)
        {
            SetClass("fade-in", this.IsEnabled());

            bool isAlive;

            // needed regarding https://github.com/Facepunch/sbox-issues/issues/1197
            try
            {
                isAlive = Player.LifeState == LifeState.Alive;
            }
            catch (Exception e)
            {
                Log.Warning(e.StackTrace);

                return;
            }

            // Network sync workaround
            if (Player.Health == 0 && isAlive)
            {
                _damageIndicatorLabel.Text = "";
            }
            else
            {
                float health = Player.Health / Player.MaxHealth * 100;
                HealthGroup healthGroup = GetHealthGroup(health);

                _damageIndicatorLabel.Style.FontColor = healthGroup.Color;
                _damageIndicatorLabel.Text = healthGroup.Title;
            }

            _nameLabel.Text = Player.Client?.Name ?? "";

            if (_role != Player.Role)
            {
                _role = Player.Role;

                _roleLabel.UpdateTranslation(new TranslationData(_role.GetTranslationKey("NAME")));
                _roleLabel.Style.FontColor = _role is NoneRole ? Color.White : _role.Color;
            }
        }
    }
}
