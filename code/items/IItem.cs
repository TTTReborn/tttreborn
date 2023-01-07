using System;

using Sandbox;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ItemAttribute : Attribute { }

    public interface IItem
    {
        static string ITEM_TAG => "TTT_ITEM";

        ItemInfo Info { get; }

        void Equip(Player player);

        void OnEquip();

        void Remove();

        void OnRemove();

        string GetTranslationKey(string key = null);

        void Delete();

        void Simulate(IClient owner);

        void OnPurchase(Player player)
        {
            player.Inventory.TryAdd(this, deleteIfFails: true, makeActive: false);
        }
    }
}
