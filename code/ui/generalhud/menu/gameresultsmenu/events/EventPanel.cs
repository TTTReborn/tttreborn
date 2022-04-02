using Sandbox.UI;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class EventPanel : Panel
    {
        private TranslationLabel TitleLabel { get; set; }
        private TranslationLabel DescriptionLabel { get; set; }

        private float CreatedAt { get; set; }

        public string FormattedCreatedAt
        {
            get => $"[{Utils.TimerString(CreatedAt)}]";
        }

        private Panel EventIcon { get; set; }

        public EventPanel(ILoggedGameEvent gameEvent)
        {
            AddClass("text-shadow");
            AddClass("eventpanel");

            CreatedAt = gameEvent.CreatedAt;

            EventIcon.Style.SetBackgroundImage(Sandbox.Texture.Load(Sandbox.FileSystem.Mounted, $"assets/events/{gameEvent.Name}.png", false) ?? Sandbox.Texture.Load(Sandbox.FileSystem.Mounted, $"assets/events/ttt_gameevent_base.png", false));
            TitleLabel?.UpdateTranslation(gameEvent.GetTitleTranslationData());
            DescriptionLabel?.UpdateTranslation(gameEvent.GetDescriptionTranslationData());
        }
    }
}
