using System;

using Sandbox;

using TTTReborn.Extensions;
using TTTReborn.Player;
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

    [Library("ttt_perk")]
    public abstract class TTTPerk : IItem
    {
        public string LibraryName { get; }
        public string DisplayName { get; }
        public Entity Owner { get; private set; }

        protected TTTPerk()
        {
            LibraryName = Library.GetAttribute(GetType()).Name;

            PerkAttribute perkAttribute = GetType().GetAttribute<PerkAttribute>();
            DisplayName = perkAttribute?.DisplayName ?? LibraryName;
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
