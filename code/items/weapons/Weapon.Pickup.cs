using Sandbox;

using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.Items
{
    public abstract partial class Weapon
    {
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

        public virtual bool CanDrop { get; set; } = true;

        public float HintDistance => 80f;

        public TranslationData TextOnTick => new("WEAPON.USE", new TranslationData(GetTranslationKey("NAME")));

        public bool CanHint(Player client) => true;

        public EntityHintPanel DisplayHint(Player client) => new GlyphHint(TextOnTick, InputButton.Use);

        public void TextTick(Player player)
        {
            if (IsClient || player.LifeState != LifeState.Alive)
            {
                return;
            }

            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    ICarriableItem[] carriableItems = player.Inventory.GetSlotCarriable(WeaponInfo.Category);

                    if (carriableItems.Length > 0)
                    {
                        ICarriableItem carriableItem = carriableItems[0];

                        if (carriableItem is Weapon weapon)
                        {
                            player.Inventory.Drop(weapon);
                        }
                    }

                    player.Inventory.TryAdd(this, deleteIfFails: false, makeActive: true);
                }
            }
        }
    }
}
