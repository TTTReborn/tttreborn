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

                if (_effectImage.Texture == null)
                {
                    _effectImage.Style.Display = DisplayMode.None;
                    _nameLabel.Style.Display = DisplayMode.Flex;
                }
                else
                {
                    _effectImage.Style.Display = DisplayMode.Flex;
                    _nameLabel.Style.Display = DisplayMode.None;
                }

                if (_item is TTTCountdownPerk countdownPerk)
                {
                    ActivateCountdown();
                }
                else
                {
                    Countdown?.Delete();
                    Countdown ??= null;
                }
            }
        }

        private IItem _item;
        private readonly Label _nameLabel;
        private readonly Image _effectImage;
        private Label Countdown;

        public Effect(Panel parent, IItem effect)
        {
            Parent = parent;

            _nameLabel = Add.Label("", "textlabel");
            _effectImage = Add.Image("", "effectimage");

            Item = effect;
        }

        private void ActivateCountdown()
        {
            Countdown = Add.Label("", "countdown");
        }

        public override void Tick()
        {
            if (Countdown != null && Item is TTTCountdownPerk countdownPerk)
            {
                Countdown.Text = $"{(countdownPerk.Countdown - countdownPerk.LastCountdown):n1}";
            }
        }
    }
}
