using Sandbox;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class InspectEntry : Panel
    {
        public string DescriptionTranslationKey;
        public object[] Data;

        private readonly Sandbox.UI.Image _inspectIcon;
        private readonly TranslationLabel _inspectQuickLabel;

        public InspectEntry(Panel parent) : base(parent)
        {
            Parent = parent;

            AddClass("rounded");
            AddClass("text-shadow");
            AddClass("background-color-secondary");

            _inspectIcon = Add.Image();
            _inspectIcon.AddClass("inspect-icon");

            _inspectQuickLabel = Add.TranslationLabel();
            _inspectQuickLabel.AddClass("quick-label");
        }

        public void SetData(string imagePath, string descriptionTranslationKey, params object[] args)
        {
            SetTranslationData(descriptionTranslationKey, args);

            _inspectIcon.Style.BackgroundImage = Texture.Load(imagePath, false) ?? Texture.Load($"/ui/none.png");
            _inspectIcon.Style.Dirty();
        }

        public void SetTranslationData(string descriptionTranslationKey, params object[] args)
        {
            DescriptionTranslationKey = descriptionTranslationKey;
            Data = args;
        }

        public void SetQuickInfo(string translationKey, params object[] args)
        {
            _inspectQuickLabel.SetTranslation(translationKey, args);
        }
    }
}
