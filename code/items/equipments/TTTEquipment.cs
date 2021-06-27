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

    public abstract class TTTEquipment : Networked, IBuyableItem
    {
        private string Name { get; set; }

        protected TTTEquipment()
        {
            EquipmentAttribute equipmentAttribute = Library.GetAttribute(GetType()) as EquipmentAttribute;

            Name = equipmentAttribute.Name;
        }

        public virtual int GetPrice() => 100;

        public virtual bool IsBuyable(TTTPlayer player) => true;

        public string GetName() => Name;

        public void Equip(TTTPlayer player)
        {

        }
    }
}
