namespace TTTReborn.UI.VisualProgramming
{
    public class Node : Modal
    {
        public Node(Sandbox.UI.Panel parent = null) : base(parent)
        {
            Header.DragHeader.IsFreeDraggable = true;
            Header.DragHeader.IsLocked = false;
        }
    }
}
