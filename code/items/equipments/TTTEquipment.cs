using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EquipmentAttribute : LibraryAttribute
    {
        public EquipmentAttribute(string name) : base(name)
        {

        }
    }

    public abstract class TTTEquipment : BaseCarriable, IItem
    {
        private string Name { get; set; }

        protected TTTEquipment()
        {
            EquipmentAttribute equipmentAttribute = Library.GetAttribute(GetType()) as EquipmentAttribute;

            Name = equipmentAttribute.Name;
        }

        public string GetName() => Name;

        public void Equip(TTTPlayer player)
        {

        }

        public virtual bool CanDrop() => true;
    }
}
