using TTTReborn.Globalization;

namespace TTTReborn
{
    public interface ILoggedGameEvent
    {
        public string Name { get; set; }

        public virtual TranslationData TitleTranslationData => new(Utils.GetTranslationKey(Name, "TITLE"));
        public TranslationData DescriptionTranslationData { get; }

        public float CreatedAt { get; set; }

        public virtual Sandbox.UI.Panel GetEventPanel() => new UI.EventPanel(this);
    }
}
