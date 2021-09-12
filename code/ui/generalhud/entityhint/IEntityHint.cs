using TTTReborn.Player;

namespace TTTReborn.UI
{
    public interface IEntityHint
    {
        bool CanHint(TTTPlayer client);
        Panel DisplayHint(TTTPlayer client);
    }
}
