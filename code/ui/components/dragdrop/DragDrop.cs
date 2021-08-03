using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class DragDrop : TTTPanel
    {
        public string DragDropGroupName { get; set; } = "";

        public DragDrop(Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;
        }
    }
}
