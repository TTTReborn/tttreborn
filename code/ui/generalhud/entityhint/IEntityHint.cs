using TTTReborn.Globalization;
using TTTReborn.Player;

namespace TTTReborn.UI
{
    public interface IEntityHint
    {
        /// <summary>
        /// The max viewable distance of the hint.
        /// </summary>
        public float HintDistance => 2048f;

        public bool ShowGlow => true;

        /// <summary>
        /// The text to display on the hint each tick.
        /// </summary>
        public TranslationData TextOnTick => null;

        public bool CanHint(TTTPlayer client);

        public EntityHintPanel DisplayHint(TTTPlayer client);

        public void StopUsing(TTTPlayer player);

        public void TickUse(TTTPlayer player);
    }
}
