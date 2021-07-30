namespace TTTReborn.UI
{
    public partial class Menu : ResizeablePanel
    {
        private DraggableHeader _draggableHeader;

        public Menu() : base()
        {
            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _draggableHeader = new DraggableHeader(this);
        }
    }
}
