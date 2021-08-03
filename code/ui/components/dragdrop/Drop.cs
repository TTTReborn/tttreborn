using Sandbox.UI;

// TODO use M4x4 transform

namespace TTTReborn.UI
{
    public partial class Drop : DragDrop
    {
        public Drop(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;
        }
    }
}
