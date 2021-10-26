using System;

namespace TTTReborn.Items
{
    public enum SlotType
    {
        Primary = 1,
        Secondary,
        Melee,
        OffensiveEquipment,
        UtilityEquipment,
        Grenade
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CarriableAttribute : ItemAttribute
    {
        public SlotType SlotType = SlotType.Primary;

        public CarriableAttribute() : base()
        {

        }
    }

    public interface ICarriableItem : IItem
    {
        SlotType SlotType { get; }

        bool CanDrop();
    }
}
