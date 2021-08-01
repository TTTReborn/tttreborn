using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class MenuHeader : DraggableHeader
    {
        public Button HomeButton { get; private set; }
        public Button PreviousButton { get; private set; }
        public Button NextButton { get; private set; }

        public MenuHeader(Menu parent) : base(parent)
        {
            Parent = parent;

            StyleSheet.Load("/ui/generalhud/menu/MenuHeader.scss");
        }

        public override void OnCreateHeader()
        {
            HomeButton = new Button("home", "", () => (Parent as Menu).OpenHomepage());
            HomeButton.AddClass("home");

            AddChild(HomeButton);

            PreviousButton = Add.Button("<", "previous", () => (Parent as Menu).MenuContent.Previous());
            NextButton = Add.Button(">", "next", () => (Parent as Menu).MenuContent.Next());

            HomeButton.SetClass("disabled", true);
            PreviousButton.SetClass("disabled", true);
            NextButton.SetClass("disabled", true);
        }
    }
}
