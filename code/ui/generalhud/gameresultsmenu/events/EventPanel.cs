using Sandbox.UI;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class EventPanel : Panel
    {
        private TranslationLabel NameLabel { get; set; }

        private string Name { get; set; }

        protected string CreatedAt { get; set; }

        private Panel EventIcon { get; set; }

        public EventPanel(GameEvent gameEvent)
        {
            AddClass("text-shadow");
            AddClass("eventpanel");

            Name = gameEvent.Name;
            CreatedAt = gameEvent.CreatedAt.ToString();

            EventIcon.Style.SetBackgroundImage(Sandbox.Texture.Load(Sandbox.FileSystem.Mounted, $"assets/events/{Name}.png", false));
            NameLabel?.UpdateTranslation(new(Utils.GetTranslationKey(Name, "NAME").Replace('_', '.')));
        }
    }
}
