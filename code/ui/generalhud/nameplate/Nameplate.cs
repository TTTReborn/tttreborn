using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Player.Camera;

namespace TTTReborn.UI
{
    public class Nameplate : TTTPanel
    {
        public static Nameplate Instance;

        private const float MAX_DRAW_DISTANCE = 500;
        private readonly Color BORDER_COLOR_NONE = Color.FromBytes(0, 0, 0, 204);

        private readonly Panel _labelHolder;
        private readonly Panel _nameHolder;
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
        private HealthGroup[] HealthGroupList = new HealthGroup[]{
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 70),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44), 20),
            new HealthGroup("Near death", Color.FromBytes(252, 42, 42), 0)
        };

        public Nameplate()
        {
            Instance = this;
            IsShowing = false;

            StyleSheet.Load("/ui/generalhud/nameplate/Nameplate.scss");

            _labelHolder = Add.Panel("labelHolder");

            _nameHolder = _labelHolder.Add.Panel("nameHolder");
            _nameLabel = _nameHolder.Add.Label("", "name");

            _damageIndicatorLabel = _labelHolder.Add.Label("", "damageIndicator");
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

            return HealthGroupList[HealthGroupList.Length - 1];
        }

        public override void Tick()
        {
            base.Tick();

            if (Local.Pawn is not TTTPlayer player || player.Camera is ThirdPersonSpectateCamera)
            {
                IsShowing = false;

                return;
            }

            TTTPlayer target = player.IsLookingAtType<TTTPlayer>(MAX_DRAW_DISTANCE);

            if (target != null)
            {
                IsShowing = true;

                if (target.Health == 0 && target.LifeState == LifeState.Alive) // network-sync workaround
                {
                    _damageIndicatorLabel.Text = "";
                }
                else
                {
                    float health = target.Health / target.MaxHealth * 100;
                    HealthGroup healthGroup = GetHealthGroup(health);

                    _damageIndicatorLabel.Style.FontColor = healthGroup.Color;
                    _damageIndicatorLabel.Text = healthGroup.Title;
                    _damageIndicatorLabel.Style.Dirty();
                }

                _nameLabel.Text = target.GetClientOwner()?.Name ?? "";

                Style.BorderColor = target.Role is not TTTReborn.Roles.NoneRole ? target.Role.Color : BORDER_COLOR_NONE;

                Style.Dirty();
            }
            else
            {
                IsShowing = false;
            }
        }
    }
}
