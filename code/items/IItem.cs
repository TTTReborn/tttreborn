using System;

using Sandbox;

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

        void Equip(Player player);

        void OnEquip();

        void Remove();

        void OnRemove();

        string GetTranslationKey(string key = null);

        void Delete();

        void Simulate(Client owner);

        void OnPurchase(Player player)
        {
            player.Inventory.TryAdd(this, deleteIfFails: true, makeActive: false);
        }
    }
}
