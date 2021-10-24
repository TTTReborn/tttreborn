using TTTReborn.Player;
using TTTReborn.UI;

namespace TTTReborn.Hints
{
    public interface IEntityHint
    {
        /// <summary>
        /// The max viewable distance of the hint.
        /// </summary>
        float HintDistance => 2048f;

        bool CanHint(TTTPlayer player);
        EntityHintPanel DisplayHintPanel(TTTPlayer player);
        void UpdateHintPanel(EntityHintPanel entityHintPanel);
    }
}
