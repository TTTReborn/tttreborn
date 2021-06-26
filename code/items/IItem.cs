using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IItem
    {
        string GetName();

        void Equip(TTTPlayer player);

        bool CanDrop();
    }
}
