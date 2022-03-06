using System;

using Sandbox;

namespace TTTReborn.Items
{
    public enum CarriableCategories
    {
        Melee,
        Pistol,
        SMG,
        Shotgun,
        Sniper,
        OffensiveEquipment,
        UtilityEquipment,
        Grenade
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CarriableAttribute : ItemAttribute
    {
        public CarriableCategories Category;

        public CarriableAttribute(CarriableCategories category) : base()
        {
            Category = category;
        }
    }

    public interface ICarriableItem : IItem, IPickupable
    {
        CarriableCategories Category { get; }

        bool CanDrop();
    }
}
