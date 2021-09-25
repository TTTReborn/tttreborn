using Sandbox;

using TTTReborn.Player;
using TTTReborn.UI;

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
        public string ClassName { get; }
        public Entity Owner { get; private set; }

        protected TTTPerk()
        {
            ClassName = Library.GetAttribute(GetType()).Name;
        }

        public void Equip(TTTPlayer player)
        {
            Owner = player;

            OnEquip();
        }

        public virtual void OnEquip()
        {
            if (Host.IsClient)
            {
                InventoryWrapper.Instance.Effects.AddEffect(this);
            }
        }

        public void Remove()
        {
            OnRemove();
        }

        public virtual void OnRemove()
        {
            Owner = null;
        }

        public void Delete()
        {
            if (Host.IsClient)
            {
                InventoryWrapper.Instance.Effects.RemoveEffect(this);
            }
        }

        public virtual void Simulate(Client owner)
        {

        }
    }
}
