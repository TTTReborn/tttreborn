namespace TTTReborn.UI
{
    public class Hint : EntityHintPanel
    {
        private readonly TranslationLabel _label;

        public Hint(TranslationLabel translationLabel)
        {
            AddClass("centered-vertical-75");
            AddClass("background-color-primary");
            AddClass("rounded");
            AddClass("text-color-info");
            AddClass("text-shadow");

            _label = translationLabel;
            _label.Style.Padding = 10;

            AddChild(_label);

            Enabled = false;
        }

        public override void UpdateHintPanel(TranslationLabel translationLabel)
        {
            if (translationLabel == null)
            {
                return;
            }

            _label.Text = translationLabel.Text;
        }
    }
}
