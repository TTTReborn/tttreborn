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
    public abstract partial class Perk : BaseNetworkable, IItem
    {
        [Net]
        public ItemInfo Info { get; set; } = new();

        public Perk() : base()
        {
            Info = new()
            {
                LibraryName = Utils.GetLibraryName(GetType())
            };
        }

        public void Equip(Player player)
        {
            Info.Owner = player;

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
            Info.Owner = null;
        }

        public string GetTranslationKey(string key = null) => Utils.GetTranslationKey(Info.LibraryName, key);

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
