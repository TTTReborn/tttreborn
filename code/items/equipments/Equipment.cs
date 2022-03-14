using System;

using Sandbox;

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
    public abstract partial class Equipment : BaseCarriable, ICarriableItem
    {
        [Net]
        public ItemInfo Info { get; set; } = new CarriableInfo();

        public CarriableInfo CarriableInfo
        {
            get => Info as CarriableInfo;
        }

        public Type ObjectType { get; }
        public PickupTrigger PickupTrigger { get; set; }
        public Entity LastDropOwner { get; set; }
        public TimeSince SinceLastDrop { get; set; } = 0f;
        public virtual bool CanDrop { get; set; } = true;

        public Equipment()
        {
            Type type = GetType();

            Info = new CarriableInfo
            {
                LibraryName = Utils.GetLibraryName(type)
            };

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
                PickupTrigger = new();
                PickupTrigger.Parent = this;
                PickupTrigger.Position = Position;
                PickupTrigger.Rotation = Rotation;
            }
        }

        public void Equip(Player player)
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

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(Info.LibraryName, key);

        public virtual void PickupStartTouch(Entity other)
        {
            if ((other != LastDropOwner || SinceLastDrop > 0.25f) && other is Player player)
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

            if (!IsServer || Owner is not Player owner || !CanDrop)
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
