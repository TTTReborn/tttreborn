using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Items;

namespace TTTReborn.UI
{
    public class Effect : Panel
    {
        public IItem Item
        {
            get
            {
                return _item;
            }
            private set
            {
                _item = value;

                _nameLabel.Text = _item?.Name ?? "";
            }
        }

        private IItem _item;
        private readonly Label _nameLabel;

        public Effect(Panel parent, IItem effect)
        {
            Parent = parent;

            _nameLabel = Add.Label("", "textlabel");

            Item = effect;
        }
    }
}
