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
        public virtual SlotType SlotType => SlotType.Equipment;

        public string ClassName { get; }

        protected TTTEquipment()
        {
            ClassName = Library.GetAttribute(GetType()).Name;
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
