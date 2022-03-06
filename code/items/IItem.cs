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

        string GetTranslationKey(string key);

        void Delete();

        void Simulate(Client owner);

        void OnPurchase(TTTPlayer player)
        {
            player.Inventory.TryAdd(this, deleteIfFails: true, makeActive: false);
        }

        public static string GetTranslationKey(string libraryName, string key)
        {
            string[] splits = libraryName.Split('_');

            return $"ITEM.{splits[0]}.{string.Join('_', splits[1..])}.{key}".ToUpper();
        }
    }
}
