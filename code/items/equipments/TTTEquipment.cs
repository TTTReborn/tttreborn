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

    [Carriable("ttt_equipment", SlotType = SlotType.UtilityEquipment)]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        protected TTTEquipment()
        {

        }

        public void Equip(TTTPlayer player)
        {
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

        public virtual bool CanDrop() => true;
    }
}
