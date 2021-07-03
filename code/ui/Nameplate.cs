using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    public class Nameplate : Panel
    {
        public static Nameplate Instance;

        public bool IsShowing
        {
            get => _isShowing;
            set
            {
                _isShowing = value;

                SetClass("hide", !_isShowing);
            }
        }

        private const float MAX_DRAW_DISTANCE = 500;
        private readonly Color BORDER_COLOR_NONE = Color.FromBytes(0, 0, 0, 204);

        private readonly Panel _labelHolder;
        private readonly Panel _nameHolder;
        private readonly Label _nameLabel;
        private readonly Label _damageIndicatorLabel;

        private bool _isShowing = false;
        private float _playerHp;

        private struct HealthGroup
        {
            public string Title;
            public Color Color;

            public HealthGroup(string title, Color color)
            {
                Title = title;
                Color = color;
            }
        }

        private HealthGroup[] HealthGroupList = new HealthGroup[]{
            new HealthGroup("Healthy", Color.FromBytes(44, 233, 44)),
            new HealthGroup("Injured", Color.FromBytes(233, 135, 44)),
            new HealthGroup("Near death", Color.FromBytes(252, 42, 42))
        };

        private enum HealthGroups
        {
            Healthy,
            Injured,
            NearDeath
        }

        public Nameplate()
        {
            Instance = this;
            IsShowing = false;

            StyleSheet.Load("/ui/Nameplate.scss");

            _labelHolder = Add.Panel("labelHolder");

            _nameHolder = _labelHolder.Add.Panel("nameHolder");
            _nameLabel = _nameHolder.Add.Label("", "name");

            _damageIndicatorLabel = _labelHolder.Add.Label("", "damageIndicator");

        }

        private HealthGroup GetHealthGroup(float health)
        {
            return health > 70 ? HealthGroupList[(int) HealthGroups.Healthy]
                : health > 20 ? HealthGroupList[(int) HealthGroups.Injured]
                : HealthGroupList[(int) HealthGroups.NearDeath];
        }

        public void SetHealth(float health)
        {
            _playerHp = health;
        }

        public override void Tick()
        {
            TTTPlayer player = Local.Pawn as TTTPlayer;

            TraceResult trace = Trace.Ray(player.EyePos, player.EyePos + player.EyeRot.Forward * MAX_DRAW_DISTANCE)
                .Ignore(player.ActiveChild)
                .Ignore(player)
                .UseHitboxes()
                .Run();

            bool validHit = false;

            if (trace.Hit && trace.Entity is TTTPlayer target)
            {
                HealthGroup healthGroup = GetHealthGroup(_playerHp);

                validHit = true;

                _nameLabel.Text = target.GetClientOwner()?.Name ?? "";
                _damageIndicatorLabel.Style.FontColor = healthGroup.Color;
                _damageIndicatorLabel.Text = healthGroup.Title;
                _damageIndicatorLabel.Style.Dirty();

                Style.BorderColor = target.Role is not TTTReborn.Roles.NoneRole ? target.Role.Color : BORDER_COLOR_NONE;

                Style.Dirty();
            }

            SetClass("hide", !validHit);
        }
    }
}
