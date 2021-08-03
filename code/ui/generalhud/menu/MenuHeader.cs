using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class MenuHeader : TTTPanel
    {
        public readonly MenuNavigationHeader NavigationHeader;

        private DragDrop _dragDropHeaderWrapper;

        public MenuHeader(Menu parent) : base(parent)
        {
            Parent = parent;

            StyleSheet.Load("/ui/generalhud/menu/MenuHeader.scss");

            _dragDropHeaderWrapper = new MenuDragDropHeader(this, Parent as Menu);

            NavigationHeader = new MenuNavigationHeader(_dragDropHeaderWrapper, Parent as Menu);
        }
    }

    public partial class MenuNavigationHeader : PanelHeader
    {
        public Button HomeButton { get; private set; }
        public Button PreviousButton { get; private set; }
        public Button NextButton { get; private set; }

        public readonly Menu Menu;

        public MenuNavigationHeader(Panel parent, Menu menu) : base(parent)
        {
            Parent = parent;
            Menu = menu;

            OnClose = (panelHeader) =>
            {
                Menu.IsShowing = false;
            };
        }

        public override void OnCreateHeader()
        {
            HomeButton = new Button("home", "", () => Menu.OpenHomepage());
            HomeButton.AddClass("home");

            AddChild(HomeButton);

            PreviousButton = Add.Button("<", "previous", () => Menu.MenuContent.Previous());
            NextButton = Add.Button(">", "next", () => Menu.MenuContent.Next());

            HomeButton.SetClass("disabled", true);
            PreviousButton.SetClass("disabled", true);
            NextButton.SetClass("disabled", true);
        }
    }

    public partial class MenuDragDropHeader : DragDrop
    {
        public readonly Menu Menu;

        public MenuDragDropHeader(Panel parent, Menu menu) : base(parent)
        {
            Parent = parent;
            Menu = menu;
        }

        public override void OnDragPanel(float left, float top)
        {
            Menu.Style.Left = Length.Pixels(left);
            Menu.Style.Top = Length.Pixels(top);

            Menu.Style.Dirty();
        }
    }
}
