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
    public abstract class Equipment : BaseCarriable, ICarriableItem
    {
        public string LibraryName { get; }
        public CarriableCategories Category { get; }
        public PickupTrigger PickupTrigger { get; set; }
        public Entity LastDropOwner { get; set; }
        public TimeSince SinceLastDrop { get; set; } = 0f;

        protected Equipment()
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

            PickupTrigger = new();
            PickupTrigger.Parent = this;
            PickupTrigger.Position = Position;
            PickupTrigger.Rotation = Rotation;
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

        public string GetTranslationKey(string key) => IItem.GetTranslationKey(LibraryName, key);

        public virtual bool CanDrop() => true;

        public virtual void PickupStartTouch(Entity other)
        {
            if (other != LastDropOwner && other is TTTPlayer player)
            {
                player.Inventory.TryAdd(this);
            }
        }

        public virtual void PickupEndTouch(Entity other)
        {
            if (other is TTTPlayer)
            {
                LastDropOwner = null;
            }
        }

        public override void OnCarryStart(Entity carrier)
        {
            base.OnCarryStart(carrier);

            if (PickupTrigger.IsValid())
            {
                PickupTrigger.EnableTouch = false;
            }
        }

        public override void OnCarryDrop(Entity dropper)
        {
            LastDropOwner = Owner;
            SinceLastDrop = 0f;

            base.OnCarryDrop(dropper);

            if (PickupTrigger.IsValid())
            {
                PickupTrigger.EnableTouch = true;
            }
        }

        public override void Simulate(Client owner)
        {
            if (SinceLastDrop > 0.5f)
            {
                LastDropOwner = null;
            }

            base.Simulate(owner);
        }
    }
}
