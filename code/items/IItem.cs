using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ItemAttribute : LibraryAttribute
    {
        public ItemAttribute(string name) : base(name)
        {

        }
    }

    public interface IItem
    {
        static string ITEM_TAG => "TTT_ITEM";

        string LibraryName
        {
            get => Library.GetAttribute(GetType()).Name;
        }

        Entity Owner { get; }

        void Equip(TTTPlayer player);

        void OnEquip();

        void Remove();

        void OnRemove();

        void Delete();

        void Simulate(Client owner);
    }
}
