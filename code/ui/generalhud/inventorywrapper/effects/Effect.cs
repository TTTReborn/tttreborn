using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class Effect : Panel
    {
        public IItem Item
        {
            get
            {
                return _item;
            }
            private set
            {
                _item = value;

                _nameLabel.Text = _item?.Name ?? "";
                _effectImage.Texture = _item != null ? Texture.Load($"/ui/weapons/{_item.Name}.png") : null;

                if (_item is TTTCountdownPerk countdownPerk)
                {
                    ActivateCountdown();
                }
                else
                {
                    _countdownLabel?.Delete();
                    _countdownLabel ??= null;
                }
            }
        }

        private IItem _item;
        private readonly Label _nameLabel;
        private readonly Image _effectImage;
        private Label _countdownLabel;

        public Effect(Sandbox.UI.Panel parent, IItem effect) : base(parent)
        {
            Parent = parent;

            AddClass("background-color-primary");
            AddClass("opacity-90");
            AddClass("rounded");

            _nameLabel = Add.Label();
            _nameLabel.AddClass("name-label");

            _effectImage = Add.Image();
            _effectImage.AddClass("effect-image");

            Item = effect;
        }

        private void ActivateCountdown()
        {
            _countdownLabel = Add.Label();
            _countdownLabel.AddClass("countdown");
            _countdownLabel.AddClass("centered");
            _countdownLabel.AddClass("text-shadow");
        }

        public override void Tick()
        {
            base.Tick();

            if (_countdownLabel != null && Item is TTTCountdownPerk countdownPerk)
            {
                int currentCountdown = (countdownPerk.Countdown - countdownPerk.LastCountdown).CeilToInt();

                if (currentCountdown == countdownPerk.Countdown.CeilToInt())
                {
                    _countdownLabel.Text = "";
                }

                _countdownLabel.Text = $"{currentCountdown:n0}";
            }
        }
    }
}
