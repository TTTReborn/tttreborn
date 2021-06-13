using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI
{
    public class QuickShop : Panel
    {
        private Header header;
        private Content content;
        private Footer footer;

        public QuickShop()
        {
            StyleSheet.Load("/ui/QuickShop.scss");

            header = new Header(this);
            content = new Content(this);
            footer = new Footer(this);
        }

        private class Header : Panel
        {
            public Label TitleLabel { get; set; }
            public Label CreditsLabel { get; set; }

            public Header(Panel parent)
            {
                Parent = parent;

                TitleLabel = Add.Label("Shop", "title");
                CreditsLabel = Add.Label("$ 0", "credits");
            }
        }

        private class Content : Panel
        {
            public Content(Panel parent)
            {
                Parent = parent;
            }
        }

        private class Footer : Panel
        {
            private Description description;
            private BuyArea buyArea;

            public Footer(Panel parent)
            {
                Parent = parent;

                description = new Description(this);
                buyArea = new BuyArea(this);
            }

            private class Description : Panel
            {
                public Label EquipmentLabel;
                public Label DescriptionLabel;

                public Description(Panel parent)
                {
                    Parent = parent;

                    EquipmentLabel = Add.Label("ItemName", "equipment");
                    DescriptionLabel = Add.Label("Some item description...", "description");
                }
            }

            private class BuyArea : Panel
            {
                public Label PriceLabel;
                public Button BuyButton;

                public BuyArea(Panel parent)
                {
                    Parent = parent;

                    PriceLabel = Add.Label("$ 200", "price");
                    BuyButton = Add.Button("Buy", "buyButton");
                }
            }
        }
    }
}
