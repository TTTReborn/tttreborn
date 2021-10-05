using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EquipmentAttribute : CarriableAttribute
    {
        public EquipmentAttribute(string name) : base(name)
        {

        }
    }

    [Equipment("ttt_equipment", SlotType = SlotType.UtilityEquipment)]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        public string LibraryName { get; }
        public SlotType SlotType { get; }

        protected TTTEquipment()
        {
            EquipmentAttribute equipmentAttribute = Library.GetAttribute(GetType()) as EquipmentAttribute;

            LibraryName = equipmentAttribute.Name;
            SlotType = equipmentAttribute.SlotType;
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
