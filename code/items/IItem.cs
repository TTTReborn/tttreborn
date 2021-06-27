using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IItem
    {
        string Name { get; }

        void Equip(TTTPlayer player);
    }
}
