using System;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;
using TTTReborn.Roles;

#pragma warning disable IDE0051

namespace TTTReborn.UI
{
    [UseTemplate]
    public class Nameplate : EntityHintPanel
    {
        public Player Player;

        private Panel LabelHolder { get; set; }
        private Panel NameHolder { get; set; }
        private Label NameLabel { get; set; }
        private Label DamageLabel { get; set; }
        private TranslationLabel RoleLabel { get; set; }
        private Role _role;

        public Nameplate(Player player) : base()
        {
            Player = player;
            _role = player.Role;

            RoleLabel.UpdateTranslation(new TranslationData(_role.GetTranslationKey("NAME")));
            RoleLabel.Style.FontColor = _role is NoneRole ? Color.White : _role.Color;
            RoleLabel.Style.TextShadow.Clear();

            this.Enabled(false);
        }

        public override void UpdateHintPanel(params TranslationData[] translationData)
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
                DamageLabel.Text = "";
            }
            else
            {
                float health = Player.Health / Player.MaxHealth * 100;
                HealthGroup healthGroup = HealthGroup.Get(health);

                DamageLabel.Style.FontColor = healthGroup.Color;
                DamageLabel.Text = healthGroup.Title;
            }

            NameLabel.Text = Player.Client?.Name ?? "";

            if (_role != Player.Role)
            {
                _role = Player.Role;

                RoleLabel.UpdateTranslation(new TranslationData(_role.GetTranslationKey("NAME")));
                RoleLabel.Style.FontColor = _role is NoneRole ? Color.White : _role.Color;
            }
        }
    }
}
