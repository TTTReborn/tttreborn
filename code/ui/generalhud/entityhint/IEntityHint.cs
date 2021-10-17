using System;

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
        /// The current string to display.
        /// </summary>
        public virtual string CurrentHintText => String.Empty;

        bool CanHint(TTTPlayer client);
        EntityHintPanel DisplayHint(TTTPlayer client);
    }
}
