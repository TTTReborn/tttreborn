using System;

using Sandbox;

using TTTReborn.Extensions;
using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EquipmentAttribute : CarriableAttribute
    {
        public EquipmentAttribute() : base()
        {

        }
    }

    [Library("ttt_equipment")]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        public string LibraryName { get; }
        public string DisplayName { get; }

        public SlotType SlotType { get; } = SlotType.UtilityEquipment;

        protected TTTEquipment()
        {
            LibraryName = Library.GetAttribute(GetType()).Name;

            EquipmentAttribute equipAttribute = GetType().GetAttribute<EquipmentAttribute>();
            DisplayName = equipAttribute?.DisplayName ?? LibraryName;

            if (equipAttribute is not null)
            {
                SlotType = equipAttribute.SlotType;
            }
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
