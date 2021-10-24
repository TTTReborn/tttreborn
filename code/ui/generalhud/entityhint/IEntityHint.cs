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
        /// The text to display on the hint each tick.
        /// </summary>
        public virtual string TextOnTick => string.Empty;

        bool CanHint(TTTPlayer client);

        EntityHintPanel DisplayHint(TTTPlayer client);
    }
}
