using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public interface IEntityHint
    {
        /// <summary>
        /// The max viewable distance of the hint.
        /// </summary>
        public float HintDistance => 2048f;

        /// <summary>
        /// If we should show a glow around the entity.
        /// </summary>
        public bool ShowGlow => true;

        /// <summary>
        /// The text to display on the hint each tick.
        /// </summary>
        public TranslationData TextOnTick => null;

        /// <summary>
        /// Whether or not we can show the UI hint.
        /// </summary>
        public bool CanHint(TTTPlayer client);

        /// <summary>
        /// The hint we should display.
        /// </summary>
        public EntityHintPanel DisplayHint(TTTPlayer client);

        /// <summary>
        /// Occurs on each tick if the hint is active.
        /// </summary>
        public void Tick(TTTPlayer player);
    }
}
