using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IItem
    {
        string Name { get; }

        void Equip(TTTPlayer player);

        void OnEquip(TTTPlayer player);

        void Remove(TTTPlayer player);

        void OnRemove(TTTPlayer player);
    }
}
