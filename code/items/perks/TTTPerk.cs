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
        public Entity Owner { get; set; }

        protected TTTPerk()
        {
            LibraryAttribute attribute = Library.GetAttribute(GetType());

            Name = attribute.Name;
        }

        public void Equip(TTTPlayer player)
        {
            Owner = player;

            OnEquip();
        }

        public virtual void OnEquip()
        {

        }

        public void Remove()
        {
            OnRemove();
        }

        public virtual void OnRemove()
        {

        }

        public void Delete()
        {

        }
    }
}
