using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    // [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    // public class PerkAttribute : LibraryAttribute
    // {
    //     public PerkAttribute(string name) : base(name)
    //     {

    //     }
    // }

    [Library("ttt_perk")]
    public abstract class TTTPerk : IItem
    {
        public string Name { get; }

        protected TTTPerk()
        {
            LibraryAttribute attribute = Library.GetAttribute(GetType());

            Name = attribute.Name;
        }

        public void Equip(TTTPlayer player)
        {
            OnEquip(player);
        }

        public virtual void OnEquip(TTTPlayer player)
        {

        }

        public void Remove(TTTPlayer player)
        {
            OnRemove(player);
        }

        public virtual void OnRemove(TTTPlayer player)
        {

        }
    }
}
