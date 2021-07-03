using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class Effect : Panel
    {
        private Label _nameLabel;
        public IItem item;

        public Effect(Panel parent)
        {
            Parent = parent;

            _nameLabel = Add.Label("", "textlabel");
        }

        public override void Tick()
        {
            base.Tick();

            if (item == null)
            {
                return;
            }

            _nameLabel.Text = item.Name;
        }
    }
}
