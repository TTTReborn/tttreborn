using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EquipmentAttribute : CarriableAttribute
    {
        public Type ObjectType { get; set; }

        public EquipmentAttribute(CarriableCategories category = CarriableCategories.UtilityEquipment) : base(category)
        {

        }
    }

    [Hammer.Skip]
    public abstract class Equipment : BaseCarriable, ICarriableItem
    {
        public string LibraryName { get; }
        public Type ObjectType { get; }
        public CarriableCategories Category { get; }
        public PickupTrigger PickupTrigger { get; set; }
        public Entity LastDropOwner { get; set; }
        public TimeSince SinceLastDrop { get; set; } = 0f;

        protected Equipment()
        {
            Type type = GetType();
            LibraryName = Utils.GetLibraryName(type);
            EquipmentAttribute equipmentAttribute = Utils.GetAttribute<EquipmentAttribute>(type);

            if (equipmentAttribute != null)
            {
                Category = equipmentAttribute.Category;
                ObjectType = equipmentAttribute.ObjectType;
            }

            EnableShadowInFirstPerson = false;
        }

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;

            if (CanDrop())
            {
                PickupTrigger = new();
                PickupTrigger.Parent = this;
                PickupTrigger.Position = Position;
                PickupTrigger.Rotation = Rotation;
            }
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

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(LibraryName, key);

        public virtual bool CanDrop() => true;

        public virtual void PickupStartTouch(Entity other)
        {
            if ((other != LastDropOwner || SinceLastDrop > 0.25f) && other is TTTPlayer player)
            {
                LastDropOwner = null;

                player.Inventory.TryAdd(this);
            }
        }

        public virtual void PickupEndTouch(Entity other)
        {
            if (other is TTTPlayer && LastDropOwner == other)
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

        public override void Simulate(Client client)
        {
            base.Simulate(client);

            if (!IsServer || Owner is not TTTPlayer owner || !CanDrop())
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Attack1))
                {
                    owner.Inventory.DropEntity(this);
                }
            }
        }
    }
}
