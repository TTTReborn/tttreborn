using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    // [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    // public class EquipmentAttribute : LibraryAttribute
    // {
    //     public EquipmentAttribute(string name) : base(name)
    //     {

    //     }
    // }

    [Library("ttt_equipment")]
    public abstract class TTTEquipment : BaseCarriable, ICarriableItem
    {
        public virtual HoldType HoldType => Items.HoldType.Melee;

        public string Name { get; }

        protected TTTEquipment()
        {
            LibraryAttribute attribute = Library.GetAttribute(GetType());

            Name = attribute.Name;
        }

        public string GetName() => Name;

        public void Equip(TTTPlayer player)
        {

        }

        public virtual bool CanDrop() => true;
    }
}
