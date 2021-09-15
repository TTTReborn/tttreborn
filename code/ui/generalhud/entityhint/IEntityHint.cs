using TTTReborn.Player;

namespace TTTReborn.UI
{
    public interface IEntityHint
    {
        bool CanHint(TTTPlayer client);
        EntityHintPanel DisplayHint(TTTPlayer client);
    }
}
