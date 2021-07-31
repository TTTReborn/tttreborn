namespace TTTReborn.UI
{
    public partial class Menu : RichPanel
    {
        public static Menu Instance;

        private DraggableHeader _draggableHeader;

        public Menu() : base()
        {
            Instance = this;

            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _draggableHeader = new DraggableHeader(this);
            _draggableHeader.SetTitle("Main Menu");

            IsDraggable = true;
        }
    }
}
