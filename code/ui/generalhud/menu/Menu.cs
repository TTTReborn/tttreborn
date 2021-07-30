using Sandbox.UI;

namespace TTTReborn.UI
{
    public partial class Menu : Panel
    {
        private DraggableHeader _draggableHeader;

        public Menu()
        {
            StyleSheet.Load("/ui/generalhud/menu/Menu.scss");

            _draggableHeader = new DraggableHeader(this);
        }
    }
}
