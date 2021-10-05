using System;

using Sandbox;

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
    public class CarriableAttribute : BuyableAttribute
    {
        public SlotType SlotType = SlotType.Primary;

        public CarriableAttribute(string name) : base(name)
        {

        }
    }

    public interface ICarriableItem : IItem
    {
        SlotType SlotType
        {
            get => (Library.GetAttribute(GetType()) as CarriableAttribute).SlotType;
        }

        bool CanDrop();
    }
}
