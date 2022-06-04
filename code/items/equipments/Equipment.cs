using System;

using Sandbox;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EquipmentAttribute : CarriableAttribute
    {
        public Type ObjectType { get; set; }

        public EquipmentAttribute(CarriableCategories category = CarriableCategories.UtilityEquipment) : base(category) { }
    }

    public abstract partial class Equipment : BaseCarriable, ICarriableItem
    {
        public ItemInfo Info { get; private set; } = new();

        public CarriableInfo CarriableInfo { get; set; } = new();

        public Type ObjectType { get; }
        public PickupTrigger PickupTrigger { get; set; }

        public Entity LastDropOwner { get; set; }

        public TimeSince TimeSinceLastDrop { get; set; } = 0f;
        public virtual bool CanDrop { get; set; } = true;

        public Equipment()
        {
            Type type = GetType();

            Info.LibraryName = Utils.GetLibraryName(type);
            EquipmentAttribute equipmentAttribute = Utils.GetAttribute<EquipmentAttribute>(type);

            if (equipmentAttribute != null)
            {
                CarriableInfo.Category = equipmentAttribute.Category;
                ObjectType = equipmentAttribute.ObjectType;
            }

            EnableShadowInFirstPerson = false;
        }

        public override void Spawn()
        {
            base.Spawn();

            RenderColor = Color.Transparent;

            if (CanDrop)
            {
                PickupTrigger = new()
                {
                    Parent = this,
                    Position = Position,
                    Rotation = Rotation
                };
            }
        }

        public void Equip(Player player)
        {
            OnEquip();
        }

        public virtual void OnEquip() { }

        public void Remove()
        {
            OnRemove();
        }

        public virtual void OnRemove() { }

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(Info.LibraryName, key);

        public virtual void PickupStartTouch(Entity other)
        {
            if ((other != LastDropOwner || TimeSinceLastDrop > 0.25f) && other is Player player)
            {
                LastDropOwner = null;

                player.Inventory.TryAdd(this);
            }
        }

        public virtual void PickupEndTouch(Entity other)
        {
            if (other is Player && LastDropOwner == other)
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
            TimeSinceLastDrop = 0f;

            base.OnCarryDrop(dropper);

            if (PickupTrigger.IsValid())
            {
                PickupTrigger.EnableTouch = true;
            }
        }

        public override void Simulate(Client client)
        {
            base.Simulate(client);

            if (!IsServer || Owner is not Player owner || !CanDrop)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.PrimaryAttack))
                {
                    owner.Inventory.DropEntity(this);
                }
            }
        }
    }
}
