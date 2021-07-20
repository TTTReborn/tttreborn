using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;
using TTTReborn.Player.Camera;

namespace TTTReborn.UI
{
    public class Nameplate : ObservablePanel
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

        public Nameplate() : base()
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

            if (ObservedPlayer == null)
            {
                return;
            }

            TTTPlayer player = Local.Pawn as TTTPlayer;

            if (IsObserving && player.Camera is ThirdPersonSpectateCamera)
            {
                return;
            }

            TraceResult trace = Trace.Ray(player.EyePos, player.EyePos + player.EyeRot.Forward * MAX_DRAW_DISTANCE)
                .Ignore(player)
                .Ignore(ObservedPlayer)
                .UseHitboxes()
                .Run();

            if (trace.Hit && trace.Entity is TTTPlayer target)
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
