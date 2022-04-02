using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class InspectEntry : Panel
    {
        public TranslationData TranslationData;
        private Image InspectIcon { get; set; }
        private TranslationLabel InspectQuickLabel { get; set; }

        public void SetData(string imagePath, TranslationData translationData)
        {
            SetTranslationData(translationData);

            InspectIcon.Style.BackgroundImage = Texture.Load(FileSystem.Mounted, imagePath, false) ?? Texture.Load(FileSystem.Mounted, $"assets/none.png");
        }

        public void SetTranslationData(TranslationData translationData)
        {
            TranslationData = translationData;
        }

        public void SetQuickInfo(TranslationData translationData)
        {
            InspectQuickLabel.UpdateTranslation(translationData);
        }
    }
}
