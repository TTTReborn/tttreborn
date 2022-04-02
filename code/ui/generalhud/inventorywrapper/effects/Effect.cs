using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Items;

namespace TTTReborn.UI
{
    [UseTemplate]
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

                NameLabel.UpdateTranslation(new TranslationData(_item != null ? _item.GetTranslationKey("NAME") : ""));
                EffectImage.Texture = _item != null ? Texture.Load(FileSystem.Mounted, $"assets/icons/{_item.Info.LibraryName}.png", false) : null;

                if (EffectImage.Texture == null)
                {
                    EffectImage.Texture = Texture.Load(FileSystem.Mounted, $"assets/none.png");
                }

                if (_item is CountdownPerk)
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
        private TranslationLabel NameLabel { get; set; }
        private Panel EffectIconPanel { get; set; }
        private Image EffectImage { get; set; }
        private Label _countdownLabel;

        public Effect(IItem effect)
        {
            Item = effect;
        }

        private void ActivateCountdown()
        {
            _countdownLabel = EffectIconPanel.Add.Label();
            _countdownLabel.AddClass("countdown");
            _countdownLabel.AddClass("centered");
            _countdownLabel.AddClass("text-shadow");
        }

        public override void Tick()
        {
            base.Tick();

            if (_countdownLabel != null && Item is CountdownPerk countdownPerk)
            {
                int currentCountdown = (countdownPerk.Countdown - countdownPerk.LastCountdown).CeilToInt();

                if (currentCountdown == countdownPerk.Countdown.CeilToInt() || currentCountdown == 0)
                {
                    EffectImage.SetClass("cooldown", false);
                    _countdownLabel.Text = "";
                }
                else
                {
                    EffectImage.SetClass("cooldown", true);
                    _countdownLabel.Text = $"{currentCountdown:n0}";
                }
            }
        }
    }
}
