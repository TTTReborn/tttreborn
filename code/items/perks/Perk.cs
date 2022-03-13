using System;

using Sandbox;

using TTTReborn.UI;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PerkAttribute : ItemAttribute
    {
        public PerkAttribute() : base()
        {

        }
    }

    [Hammer.Skip]
    public abstract class Perk : IItem
    {
        public string LibraryName { get; }
        public Entity Owner { get; private set; }

        protected Perk()
        {
            LibraryName = Utils.GetLibraryName(GetType());
        }

        public void Equip(Player player)
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

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(LibraryName, key);

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
