using TTTReborn.Player;

namespace TTTReborn.UI
{
    public interface IEntityHint
    {
        /// <summary>
        /// The max viewable distance of the hint.
        /// </summary>
        public virtual float HintDistance => 2048f;
        /// <summary>
        /// The current translation label to display.
        /// </summary>
        public virtual TranslationLabel CurrentTranslationLabel => null;

        bool CanHint(TTTPlayer client);
        EntityHintPanel DisplayHint(TTTPlayer client);
    }
}
