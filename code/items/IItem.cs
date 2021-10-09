using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ItemAttribute : Attribute
    {
        public ItemAttribute() : base()
        {

        }
    }

    public interface IItem
    {
        static string ITEM_TAG => "TTT_ITEM";

        string LibraryName { get; }

        Entity Owner { get; }

        void Equip(TTTPlayer player);

        void OnEquip();

        void Remove();

        void OnRemove();

        void Delete();

        void Simulate(Client owner);

        void OnPurchase(TTTPlayer player)
        {
            player.Inventory.TryAdd(this);
        }
    }
}
