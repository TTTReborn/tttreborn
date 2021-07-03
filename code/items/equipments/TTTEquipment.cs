using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    // [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    // public class EquipmentAttribute : LibraryAttribute
    // {
    //     public EquipmentAttribute(string name) : base(name)
    //     {

    //     }
    // }

    [Library("ttt_equipment")]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        public virtual HoldType HoldType => Items.HoldType.Melee;

        public string Name { get; }

        protected TTTEquipment()
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

        public virtual bool CanDrop() => true;

        public virtual bool IsBuyable(TTTPlayer player)
        {
            return !(player.Inventory as Inventory).IsCarryingType(GetType());
        }
    }
}
