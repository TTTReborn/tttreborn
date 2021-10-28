using TTTReborn.Player;

namespace TTTReborn.UI
{
    public interface IEntityHint
    {
        float HintDistance { get; }
        bool CanHint(TTTPlayer client);
        EntityHintPanel DisplayHint(TTTPlayer client);
    }
}
