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

        private bool _isShowing = false;

        private const float MAX_DRAW_DISTANCE = 500;

        private readonly Panel _labelHolder;
        private readonly Panel _nameHolder;
        // private readonly Label _roleColorDotLabel;
        private readonly Label _nameLabel;
        private readonly Label _damageIndicatorLabel;

        private float _playerHp;

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

        private string GetHealthGroup(float health)
        {
            return health > 70 ? "Healthy"
                : health > 20 ? "Injured"
                : "Near death";
        }

        private static Color GetHealthColor(float health)
        {
            Color healthy = Color.FromBytes(44, 233, 44);
            Color injured = Color.FromBytes(233, 135, 44);
            Color near_death = Color.FromBytes(252, 42, 42);

            return health > 70 ? healthy
                : health > 20 ? injured
                : near_death;
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
                validHit = true;
                _nameLabel.Text = target.GetClientOwner()?.Name ?? "";
                _damageIndicatorLabel.Style.FontColor = GetHealthColor(_playerHp);
                _damageIndicatorLabel.Text = GetHealthGroup(_playerHp);
                _damageIndicatorLabel.Style.Dirty();

                Style.BorderColor = target.Role is not TTTReborn.Roles.NoneRole ? target.Role.Color : Color.FromBytes(0,0,0,204);

                Style.Dirty();
            }

            SetClass("hide", !validHit);
        }
    }
}
