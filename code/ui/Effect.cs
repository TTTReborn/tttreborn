using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class Effect : Panel
    {
        public IItem Item { get; private set; }
        private readonly Label _nameLabel;

        public Effect(Panel parent, IItem effect)
        {
            Parent = parent;
            Item = effect;

            _nameLabel = Add.Label("", "textlabel");
        }

        public override void Tick()
        {
            base.Tick();

            _nameLabel.Text = Item.Name;
        }
    }
}
