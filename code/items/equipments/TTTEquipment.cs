using System;
using Sandbox;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class EquipmentAttribute : LibraryAttribute
    {
        public EquipmentAttribute(string name) : base(name)
        {

        }
    }

    public abstract class TTTEquipment : Networked, IBuyableItem
    {
        public string Name { get; private set; }

        public TTTEquipment()
        {
            EquipmentAttribute equipmentAttribute = Library.GetAttribute(GetType()) as EquipmentAttribute;

            Name = equipmentAttribute.Name;
        }

        public int GetPrice() => 100;

        public bool IsBuyable() => true;

        public string GetName() => Name;
    }
}
