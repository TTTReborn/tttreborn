namespace TTTReborn.UI
{
    public class Window : RichPanel
    {
        public WindowHeader Header { get; set; }
        public PanelContent Content { get; set; }
        public Panel Footer { get; set; }

        public Window(Sandbox.UI.Panel parent = null) : base(parent)
        {
            AddClass("window");
            AddClass("rounded");
            AddClass("text-shadow");

            Header = new(this);
            Header.AddClass("rounded-top");
            Header.AddClass("background-color-secondary");

            Content = new(this);
            Content.AddClass("background-color-primary");
            Content.AddClass("content");

            Footer = new(this);
            Footer.AddClass("footer");
            Footer.AddClass("rounded-bottom");
            Footer.AddClass("background-color-secondary");

            IsDraggable = true;
        }
    }
}
