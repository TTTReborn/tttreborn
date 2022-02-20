using TTTReborn.Globalization;

namespace TTTReborn.UI
{
    /// <summary>
    /// Implement this interface to let UI components know they need to be
    /// updated on language change. Make sure to add/remove the component to TTTLanguage.Translatables
    /// on construction and destruction.
    /// </summary>
    public interface ITranslatable
    {
        void UpdateLanguage(Language language);
    }
}
