using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class DragDrop : Panel
    {
        public string DragDropGroupName { get; set; } = "";

        public DragDrop(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Parent = parent ?? Parent;
        }
    }
}
