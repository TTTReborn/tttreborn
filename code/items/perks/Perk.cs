using System;

using Sandbox;

using TTTReborn.UI;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PerkAttribute : ItemAttribute { }

    public abstract partial class Perk : BaseNetworkable, IItem
    {
        public ItemInfo Info { get; private set; } = new();

        public Perk() : base()
        {
            Info.LibraryName = Utils.GetLibraryName(GetType());
        }

        public void Equip(Player player)
        {
            Info.Owner = player;

            OnEquip();
        }

        public virtual void OnEquip()
        {
            if (Game.IsClient)
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
            if (Game.IsClient)
            {
                InventoryWrapper.Instance.Effects.RemoveEffect(this);
            }
        }

        public virtual void Simulate(IClient owner) { }
    }
}
