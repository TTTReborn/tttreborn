using TTTReborn.Player;

namespace TTTReborn.UI
{
    public interface IEntityHint
    {
        public virtual float HintDistance => 2048f;
        bool CanHint(TTTPlayer client);
        EntityHintPanel DisplayHint(TTTPlayer client);
    }
}
