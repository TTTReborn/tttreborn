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

                _nameLabel.Text = _item?.LibraryName ?? "";
                _effectImage.Texture = (_item != null ? Texture.Load($"/ui/weapons/{_item.LibraryName}.png", false) : null);

                if (_effectImage.Texture == null)
                {
                    _effectImage.Texture = Texture.Load($"/ui/none.png");
                }

                if (_item is TTTCountdownPerk countdownPerk)
                {
                    ActivateCountdown();
                }
                else
                {
                    _countdownLabel?.Delete();
                }
            }
        }

        private IItem _item;
        private readonly Label _nameLabel;
        private readonly Panel _effectIconPanel;
        private readonly Image _effectImage;
        private Label _countdownLabel;

        public Effect(Sandbox.UI.Panel parent, IItem effect) : base(parent)
        {
            Parent = parent;

            AddClass("text-shadow");

            _effectIconPanel = new Panel(this);
            _effectIconPanel.AddClass("effect-icon-panel");

            _effectImage = _effectIconPanel.Add.Image();
            _effectImage.AddClass("effect-image");

            _nameLabel = Add.Label();
            _nameLabel.AddClass("name-label");

            Item = effect;
        }

        private void ActivateCountdown()
        {
            _countdownLabel = _effectIconPanel.Add.Label();
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

                if (currentCountdown == countdownPerk.Countdown.CeilToInt() || currentCountdown == 0)
                {
                    _effectImage.SetClass("cooldown", false);
                    _countdownLabel.Text = "";
                }
                else
                {
                    _effectImage.SetClass("cooldown", true);
                    _countdownLabel.Text = $"{currentCountdown:n0}";
                }
            }
        }
    }
}
