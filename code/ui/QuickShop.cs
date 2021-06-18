using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Equipment;

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

        public override void Tick()
        {
            base.Tick();

            SetClass("hide", !Input.Down(InputButton.Menu));
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
            private List<EquipmentPanel> equipmentPanels = new();

            private Panel wrapper;

            public Content(Panel parent)
            {
                Parent = parent;

                wrapper = Add.Panel("wrapper");

                for (int i = 0; i < 5; i++)
                {
                    AddEquipment();
                }
            }

            public void AddEquipment()
            {
                EquipmentPanel equipmentPanel = new EquipmentPanel(wrapper);
                equipmentPanel.Equipment = new TTTEquipment();

                equipmentPanels.Add(equipmentPanel);
            }

            private class EquipmentPanel : Panel
            {
                public TTTEquipment Equipment;

                public Panel ImagePanel;

                public Label EquipmentLabel;

                public EquipmentPanel(Panel parent)
                {
                    Parent = parent;

                    ImagePanel = Add.Panel("image");
                    EquipmentLabel = Add.Label("Equipment", "equipment");
                }
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
