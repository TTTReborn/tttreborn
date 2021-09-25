using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IItem
    {
        string ClassName { get; }

        Entity Owner { get; }

        void Equip(TTTPlayer player);

        void OnEquip();

        void Remove();

        void OnRemove();

        void Delete();

        void Simulate(Client owner);
    }
}
