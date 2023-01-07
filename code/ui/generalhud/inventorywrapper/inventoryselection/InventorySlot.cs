using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;
using TTTReborn.Items;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class InventorySlot : Panel
    {
        public ICarriableItem Carriable { get; init; }
        public Label SlotLabel { get; set; }

        private TranslationLabel NameLabel { get; set; }
        private Label AmmoLabel { get; set; }

        public InventorySlot(ICarriableItem carriable)
        {
            Carriable = carriable;

            SlotLabel.Text = Inventory.GetSlotByCategory(carriable.CarriableInfo.Category).ToString();

            NameLabel.UpdateTranslation(new TranslationData(Utils.GetTranslationKey(carriable.Info.LibraryName, "NAME")));

            if (Game.LocalPawn is Player player)
            {
                if (carriable is Weapon weapon && carriable.CarriableInfo.Category != CarriableCategories.Melee)
                {
                    AmmoLabel.Text = InventorySelection.FormatAmmo(weapon, player.Inventory);
                }
            }
        }

        public override void Tick()
        {
            base.Tick();

            if (Game.LocalPawn is Player player)
            {
                SlotLabel.Style.BackgroundColor = player.Team.Color;
            }
        }

        public void UpdateAmmo(string ammoText)
        {
            AmmoLabel.Text = ammoText;
        }
    }
}
