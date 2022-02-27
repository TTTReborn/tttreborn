using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EquipmentAttribute : CarriableAttribute
    {
        public EquipmentAttribute(CarriableCategories category = CarriableCategories.UtilityEquipment) : base(category)
        {

        }
    }

    [Hammer.Skip]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        public string LibraryName { get; }
        public CarriableCategories Category { get; }

        protected TTTEquipment()
        {
            LibraryName = Utils.GetLibraryName(GetType());

            foreach (object obj in GetType().GetCustomAttributes(false))
            {
                if (obj is EquipmentAttribute equipmentAttribute)
                {
                    Category = equipmentAttribute.Category;
                }
            }

            EnableShadowInFirstPerson = false;
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
