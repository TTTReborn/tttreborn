using TTTReborn.Globalization;

namespace TTTReborn
{
    public interface ILoggedGameEvent
    {
        public string Name { get; set; }

        public virtual TranslationData GetTitleTranslationData() => new(Utils.GetTranslationKey(Name, "TITLE"));
        public virtual TranslationData GetDescriptionTranslationData() => null;

        public float CreatedAt { get; set; }

        public virtual Sandbox.UI.Panel GetEventPanel() => new UI.EventPanel(this);
    }
}
