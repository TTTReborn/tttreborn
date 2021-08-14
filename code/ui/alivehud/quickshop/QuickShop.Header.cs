using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Player;

namespace TTTReborn.UI
{
    public partial class QuickShop
    {
        private class Header : TTTPanel
        {
            public Panel PriceHolder { get; set; }
            public Label DollarSignLabel;
            private Label _titleLabel;
            private readonly Label _creditsLabel;

            public Header(Panel parent)
            {
                Parent = parent;

                _titleLabel = Add.Label("Shop", "title");
                PriceHolder = Add.Panel("priceholder");
                DollarSignLabel = PriceHolder.Add.Label("$", "dollarsign");
                _creditsLabel = PriceHolder.Add.Label("0", "credits");
            }

            public override void Tick()
            {
                _creditsLabel.Text = $"{(Local.Pawn as TTTPlayer).Credits}";
            }
        }
    }
}
