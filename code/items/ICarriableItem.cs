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
    public class CarriableAttribute : BuyableItemAttribute
    {
        public SlotType SlotType = SlotType.Primary;

        public CarriableAttribute(string name) : base(name)
        {

        }
    }

    public interface ICarriableItem : IItem
    {
        SlotType SlotType { get; }

        bool CanDrop();
    }
}
